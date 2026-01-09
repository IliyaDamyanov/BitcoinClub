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

            var breezApi = new PaidBreezApiClient(now.ToUnixTimeSeconds());
            var service = new BreezPaymentService(breezApi, db);

            var init = await service.InitiateMembershipPaymentAsync(userId, 1000, "Membership");

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SubscriptionId = init.SubscriptionId,
                Provider = "breez",
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

            // Simulate controller updating payment row
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

        private static ApplicationDbContext CreateInMemoryDb()
        {
            var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
                .Options;

            return new ApplicationDbContext(opts);
        }

        private sealed class PaidBreezApiClient : IBreezApiClient
        {
            private readonly long _paidAt;

            public PaidBreezApiClient(long paidAt)
            {
                _paidAt = paidAt;
            }

            public Task<BreezPaymentInitResponse> CreateInvoiceAsync(int amountSats, string description, CancellationToken cancellationToken = default)
                => Task.FromResult(new BreezPaymentInitResponse("prov-1", "bolt11"));

            public Task<BreezPaymentStatusResponse> GetPaymentStatusAsync(string paymentId, CancellationToken cancellationToken = default)
                => Task.FromResult(new BreezPaymentStatusResponse(IsPaid: true, PaidAtUnixSeconds: _paidAt));
        }
    }
}
