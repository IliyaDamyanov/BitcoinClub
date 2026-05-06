using System;
using System.Threading;
using System.Threading.Tasks;
using BitcoinClub.Data;
using BitcoinClub.Models;
using Microsoft.EntityFrameworkCore;

namespace BitcoinClub.Infrastructure.Payments
{
    public sealed class LightningPaymentService : IPaymentService
    {
        private readonly ILightningApiClient _lightning;
        private readonly ApplicationDbContext _db;

        public LightningPaymentService(ILightningApiClient lightning, ApplicationDbContext db)
        {
            _lightning = lightning;
            _db = db;
        }

        public async Task<PaymentInitiationResult> InitiateMembershipPaymentAsync(
            string userId,
            int amountSats,
            string description,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId is required.", nameof(userId));

            if (amountSats <= 0)
                throw new ArgumentOutOfRangeException(nameof(amountSats), "Amount must be > 0 sats.");

            var invoice = await _lightning.CreateInvoiceAsync(amountSats, description, cancellationToken);

            var subscription = await _db.Subscriptions.SingleOrDefaultAsync(s => s.UserId == userId, cancellationToken);
            if (subscription is null)
            {
                subscription = new Subscription
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    ExpirationDate = DateTime.UtcNow
                };
                await _db.Subscriptions.AddAsync(subscription, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);
            }

            return new PaymentInitiationResult(
                subscription.Id,
                invoice.PaymentHash,
                invoice.PaymentRequest,
                amountSats,
                invoice.PaymentUrl,
                invoice.ExpiresAt);
        }

        public async Task<PaymentVerificationResult> VerifyPaymentAsync(
            Guid subscriptionId,
            string paymentId,
            CancellationToken cancellationToken = default)
        {
            if (subscriptionId == Guid.Empty)
                throw new ArgumentException("SubscriptionId is required.", nameof(subscriptionId));

            if (string.IsNullOrWhiteSpace(paymentId))
                throw new ArgumentException("PaymentId is required.", nameof(paymentId));

            var status = await _lightning.GetPaymentStatusAsync(paymentId, cancellationToken);
            if (!status.IsPaid)
                return new PaymentVerificationResult(false, null, null);

            var paidAt = status.PaidAt
                ?? (status.PaidAtUnixSeconds is long unix
                    ? DateTimeOffset.FromUnixTimeSeconds(unix)
                    : DateTimeOffset.UtcNow);

            return await CompletePaymentAsync(subscriptionId, paymentId, paidAt, cancellationToken);
        }

        public async Task<PaymentVerificationResult> CompleteProviderPaymentAsync(
            string provider,
            string paymentId,
            DateTimeOffset? paidAt,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(provider))
                throw new ArgumentException("Provider is required.", nameof(provider));

            if (string.IsNullOrWhiteSpace(paymentId))
                throw new ArgumentException("PaymentId is required.", nameof(paymentId));

            var payment = await _db.Payments
                .Include(p => p.Subscription)
                .SingleOrDefaultAsync(p => p.Provider == provider && p.ProviderPaymentId == paymentId, cancellationToken);

            if (payment is null)
                return new PaymentVerificationResult(false, null, null);

            return await CompletePaymentAsync(payment.SubscriptionId, paymentId, paidAt, cancellationToken);
        }

        private async Task<PaymentVerificationResult> CompletePaymentAsync(
            Guid subscriptionId,
            string paymentId,
            DateTimeOffset? paidAt,
            CancellationToken cancellationToken)
        {
            var completedAt = paidAt ?? DateTimeOffset.UtcNow;

            var payment = await _db.Payments
                .Include(p => p.Subscription)
                .SingleOrDefaultAsync(p => p.SubscriptionId == subscriptionId && p.ProviderPaymentId == paymentId, cancellationToken);

            if (payment is not null && string.Equals(payment.Status, "paid", StringComparison.OrdinalIgnoreCase))
            {
                if (payment.PaidAt is null)
                {
                    payment.PaidAt = completedAt.UtcDateTime;
                    await _db.SaveChangesAsync(cancellationToken);
                }

                var currentExpiration = payment.Subscription?.ExpirationDate
                    ?? await _db.Subscriptions
                        .Where(s => s.Id == subscriptionId)
                        .Select(s => s.ExpirationDate)
                        .SingleAsync(cancellationToken);

                return new PaymentVerificationResult(true, completedAt, currentExpiration);
            }

            var subscription = payment?.Subscription
                ?? await _db.Subscriptions.SingleAsync(s => s.Id == subscriptionId, cancellationToken);

            if (payment is not null)
            {
                payment.Status = "paid";
                payment.PaidAt = completedAt.UtcDateTime;
            }

            subscription.LastPaymentDate = completedAt.UtcDateTime;
            var baseDate = subscription.ExpirationDate > DateTime.UtcNow ? subscription.ExpirationDate : DateTime.UtcNow;
            subscription.ExpirationDate = baseDate.AddMonths(1);

            await _db.SaveChangesAsync(cancellationToken);

            return new PaymentVerificationResult(true, completedAt, subscription.ExpirationDate);
        }
    }
}
