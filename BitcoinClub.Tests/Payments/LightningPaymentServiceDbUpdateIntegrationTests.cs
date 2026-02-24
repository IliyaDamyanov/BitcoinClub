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
    public class LightningPaymentServiceDbUpdateIntegrationTests
    {
        [Fact]
        public async Task VerifyPaymentAsync_Paid_UpdatesLastPaymentDate_AndExtendsExpiration()
        {
            var now = DateTimeOffset.UtcNow;
            var lightning = new PaidLightningApiClient(now.ToUnixTimeSeconds());

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

            var sut = new LightningPaymentService(lightning, db);

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

        private sealed class PaidLightningApiClient : ILightningApiClient
        {
            private readonly long _paidAt;

            public PaidLightningApiClient(long paidAt)
            {
                _paidAt = paidAt;
            }

            public Task<LightningInvoiceResponse> CreateInvoiceAsync(int amountSats, string description, CancellationToken cancellationToken = default)
                => Task.FromResult(new LightningInvoiceResponse("invoice-1", "bolt11"));

            public Task<LightningPaymentStatusResponse> GetPaymentStatusAsync(string paymentHash, CancellationToken cancellationToken = default)
                => Task.FromResult(new LightningPaymentStatusResponse(IsPaid: true, PaidAtUnixSeconds: _paidAt));
        }
    }
}
