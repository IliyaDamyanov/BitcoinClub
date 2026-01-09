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
    public class BreezPaymentServiceDbUpdateIntegrationTests
    {
        [Fact]
        public async Task VerifyPaymentAsync_Paid_UpdatesLastPaymentDate_AndExtendsExpiration()
        {
            var now = DateTimeOffset.UtcNow;
            var breez = new PaidBreezApiClient(now.ToUnixTimeSeconds());

            var db = CreateInMemoryDb();
            var sub = new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = "user-1",
                CreatedAt = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(-1)
            };
            db.Subscriptions.Add(sub);
            await db.SaveChangesAsync();

            var sut = new BreezPaymentService(breez, db);

            var res = await sut.VerifyPaymentAsync(sub.Id, paymentId: "pay-1");

            Assert.True(res.IsPaid);
            Assert.NotNull(res.PaidAt);
            Assert.NotNull(res.NewExpirationDate);

            var reloaded = await db.Subscriptions.SingleAsync(s => s.Id == sub.Id);
            Assert.NotNull(reloaded.LastPaymentDate);
            Assert.True(reloaded.ExpirationDate > DateTime.UtcNow);
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
                => Task.FromResult(new BreezPaymentInitResponse("invoice-1", "bolt11"));

            public Task<BreezPaymentStatusResponse> GetPaymentStatusAsync(string paymentId, CancellationToken cancellationToken = default)
                => Task.FromResult(new BreezPaymentStatusResponse(IsPaid: true, PaidAtUnixSeconds: _paidAt));
        }
    }
}
