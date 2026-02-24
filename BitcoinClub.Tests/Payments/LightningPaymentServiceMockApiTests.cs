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
    public class LightningPaymentServiceMockApiTests
    {
        [Fact]
        public async Task VerifyPaymentAsync_NotPaid_DoesNotUpdateSubscription()
        {
            var mock = new RecordingLightningApiClient(isPaid: false);
            var db = CreateInMemoryDb();
            var sub = new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = "user-1",
                CreatedAt = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(1)
            };
            db.Subscriptions.Add(sub);
            await db.SaveChangesAsync();

            var sut = new LightningPaymentService(mock, db);

            var res = await sut.VerifyPaymentAsync(sub.Id, paymentId: "pay-1");

            Assert.False(res.IsPaid);
            Assert.Null(res.PaidAt);
            Assert.Null(res.NewExpirationDate);
            Assert.Equal("pay-1", mock.LastPaymentHash);

            var reloaded = await db.Subscriptions.SingleAsync(s => s.Id == sub.Id);
            Assert.Null(reloaded.LastPaymentDate);
            Assert.Equal(sub.ExpirationDate, reloaded.ExpirationDate);
        }

        private static ApplicationDbContext CreateInMemoryDb()
        {
            var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
                .Options;

            return new ApplicationDbContext(opts);
        }

        private sealed class RecordingLightningApiClient : ILightningApiClient
        {
            private readonly bool _isPaid;

            public RecordingLightningApiClient(bool isPaid)
            {
                _isPaid = isPaid;
            }

            public string? LastPaymentHash { get; private set; }

            public Task<LightningInvoiceResponse> CreateInvoiceAsync(int amountSats, string description, CancellationToken cancellationToken = default)
                => Task.FromResult(new LightningInvoiceResponse("invoice-1", "bolt11"));

            public Task<LightningPaymentStatusResponse> GetPaymentStatusAsync(string paymentHash, CancellationToken cancellationToken = default)
            {
                LastPaymentHash = paymentHash;
                return Task.FromResult(new LightningPaymentStatusResponse(_isPaid, PaidAtUnixSeconds: null));
            }
        }
    }
}
