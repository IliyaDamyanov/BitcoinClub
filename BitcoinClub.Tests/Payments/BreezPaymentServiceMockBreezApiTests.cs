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
    public class BreezPaymentServiceMockBreezApiTests
    {
        [Fact]
        public async Task VerifyPaymentAsync_NotPaid_DoesNotUpdateSubscription()
        {
            var mock = new RecordingBreezApiClient(isPaid: false);
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

            var sut = new BreezPaymentService(mock, db);

            var res = await sut.VerifyPaymentAsync(sub.Id, paymentId: "pay-1");

            Assert.False(res.IsPaid);
            Assert.Null(res.PaidAt);
            Assert.Null(res.NewExpirationDate);
            Assert.Equal("pay-1", mock.LastPaymentId);

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

        private sealed class RecordingBreezApiClient : IBreezApiClient
        {
            private readonly bool _isPaid;

            public RecordingBreezApiClient(bool isPaid)
            {
                _isPaid = isPaid;
            }

            public string? LastPaymentId { get; private set; }

            public Task<BreezPaymentInitResponse> CreateInvoiceAsync(int amountSats, string description, CancellationToken cancellationToken = default)
                => Task.FromResult(new BreezPaymentInitResponse("invoice-1", "bolt11"));

            public Task<BreezPaymentStatusResponse> GetPaymentStatusAsync(string paymentId, CancellationToken cancellationToken = default)
            {
                LastPaymentId = paymentId;
                return Task.FromResult(new BreezPaymentStatusResponse(_isPaid, PaidAtUnixSeconds: null));
            }
        }
    }
}
