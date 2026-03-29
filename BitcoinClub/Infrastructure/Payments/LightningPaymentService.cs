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

            return new PaymentInitiationResult(subscription.Id, invoice.PaymentHash, invoice.PaymentRequest, amountSats);
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

            var subscription = await _db.Subscriptions.SingleAsync(s => s.Id == subscriptionId, cancellationToken);

            var paidAt = status.PaidAtUnixSeconds is long unix
                ? DateTimeOffset.FromUnixTimeSeconds(unix)
                : DateTimeOffset.UtcNow;

            subscription.LastPaymentDate = paidAt.UtcDateTime;
            var baseDate = subscription.ExpirationDate > DateTime.UtcNow ? subscription.ExpirationDate : DateTime.UtcNow;
            subscription.ExpirationDate = baseDate.AddMonths(1);

            await _db.SaveChangesAsync(cancellationToken);

            return new PaymentVerificationResult(true, paidAt, subscription.ExpirationDate);
        }
    }
}
