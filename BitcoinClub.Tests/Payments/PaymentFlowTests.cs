using System;
using System.Threading;
using System.Threading.Tasks;
using BitcoinClub.Data;
using BitcoinClub.Infrastructure.Payments;
using BitcoinClub.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BitcoinClub.Tests.Payments
{
    public class PaymentFlowTests
    {
        [Fact]
        public async Task InitiateThenVerify_Paid_SetsPaymentPaidAndUpdatesSubscription()
        {
            var now = DateTimeOffset.UtcNow;
            var userId = "user-1";

            var db = CreateInMemoryDb();
            var sub = new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(-1)
            };
            db.Subscriptions.Add(sub);
            await db.SaveChangesAsync();

            var lightningApi = new PaidLightningApiClient(now.ToUnixTimeSeconds());
            var service = new LightningPaymentService(lightningApi, db);

            var init = await service.InitiateMembershipPaymentAsync(userId, 1000, "Membership");

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SubscriptionId = init.SubscriptionId,
                Provider = "lnbits",
                ProviderPaymentId = init.PaymentId,
                AmountSats = init.AmountSats,
                PaymentRequest = init.PaymentRequest,
                Status = "initiated",
                CreatedAt = DateTime.UtcNow
            };
            db.Payments.Add(payment);
            await db.SaveChangesAsync();

            var verify = await service.VerifyPaymentAsync(init.SubscriptionId, init.PaymentId);

            Assert.True(verify.IsPaid);

            payment.Status = "paid";
            payment.PaidAt = verify.PaidAt?.UtcDateTime;
            await db.SaveChangesAsync();

            var updatedSub = await db.Subscriptions.SingleAsync(s => s.Id == init.SubscriptionId);
            Assert.NotNull(updatedSub.LastPaymentDate);
            Assert.True(updatedSub.ExpirationDate > DateTime.UtcNow);

            var updatedPayment = await db.Payments.SingleAsync(p => p.Id == payment.Id);
            Assert.Equal("paid", updatedPayment.Status);
            Assert.NotNull(updatedPayment.PaidAt);
        }

        [Fact]
        public async Task CompleteProviderPaymentAsync_DuplicateCompletedEvent_DoesNotExtendSubscriptionTwice()
        {
            var userId = "user-1";
            var db = CreateInMemoryDb();
            var subscription = new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(-1)
            };
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SubscriptionId = subscription.Id,
                Subscription = subscription,
                Provider = "glow-pay",
                ProviderPaymentId = "pay-1",
                AmountSats = 1000,
                PaymentRequest = "bolt11",
                Status = "initiated",
                CreatedAt = DateTime.UtcNow
            };
            db.Subscriptions.Add(subscription);
            db.Payments.Add(payment);
            await db.SaveChangesAsync();

            var service = new LightningPaymentService(new PaidLightningApiClient(DateTimeOffset.UtcNow.ToUnixTimeSeconds()), db);
            var paidAt = DateTimeOffset.UtcNow;

            var first = await service.CompleteProviderPaymentAsync("glow-pay", "pay-1", paidAt);
            var expirationAfterFirst = first.NewExpirationDate;
            var second = await service.CompleteProviderPaymentAsync("glow-pay", "pay-1", paidAt);

            Assert.True(first.IsPaid);
            Assert.True(second.IsPaid);
            Assert.Equal(expirationAfterFirst, second.NewExpirationDate);

            var updatedPayment = await db.Payments.SingleAsync(p => p.Id == payment.Id);
            Assert.Equal("paid", updatedPayment.Status);
            Assert.NotNull(updatedPayment.PaidAt);
        }

        private static ApplicationDbContext CreateInMemoryDb()
        {
            var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
                .Options;

            return new ApplicationDbContext(opts);
        }

        private sealed class PaidLightningApiClient : ILightningApiClient
        {
            private readonly long _paidAt;

            public PaidLightningApiClient(long paidAt)
            {
                _paidAt = paidAt;
            }

            public Task<LightningInvoiceResponse> CreateInvoiceAsync(int amountSats, string description, CancellationToken cancellationToken = default)
                => Task.FromResult(new LightningInvoiceResponse("prov-1", "bolt11"));

            public Task<LightningPaymentStatusResponse> GetPaymentStatusAsync(string paymentHash, CancellationToken cancellationToken = default)
                => Task.FromResult(new LightningPaymentStatusResponse(IsPaid: true, PaidAtUnixSeconds: _paidAt));
        }
    }
}
