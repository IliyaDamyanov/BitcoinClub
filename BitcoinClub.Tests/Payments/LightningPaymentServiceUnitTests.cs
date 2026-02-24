using System;
using System.Threading;
using System.Threading.Tasks;
using BitcoinClub.Data;
using BitcoinClub.Infrastructure.Payments;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BitcoinClub.Tests.Payments
{
    public class LightningPaymentServiceUnitTests
    {
        [Fact]
        public async Task InitiateMembershipPaymentAsync_ValidInput_CreatesSubscription_AndReturnsInvoiceData()
        {
            var lightning = new FakeLightningApiClient(
                createInvoice: () => new LightningInvoiceResponse("p1", "bolt11"),
                getStatus: _ => new LightningPaymentStatusResponse(false, null));

            var db = CreateInMemoryDb();
            var sut = new LightningPaymentService(lightning, db);

            var res = await sut.InitiateMembershipPaymentAsync("user-1", 1000, "Membership");

            Assert.Equal("p1", res.PaymentId);
            Assert.Equal("bolt11", res.PaymentRequest);
            Assert.Equal(1000, res.AmountSats);

            var sub = await db.Subscriptions.SingleAsync(s => s.UserId == "user-1");
            Assert.Equal(res.SubscriptionId, sub.Id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task InitiateMembershipPaymentAsync_InvalidAmount_Throws(int amount)
        {
            var lightning = new FakeLightningApiClient(
                createInvoice: () => new LightningInvoiceResponse("p1", "bolt11"),
                getStatus: _ => new LightningPaymentStatusResponse(false, null));

            var db = CreateInMemoryDb();
            var sut = new LightningPaymentService(lightning, db);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                sut.InitiateMembershipPaymentAsync("user-1", amount, "Membership"));
        }

        private static ApplicationDbContext CreateInMemoryDb()
        {
            var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
                .Options;

            return new ApplicationDbContext(opts);
        }

        private sealed class FakeLightningApiClient : ILightningApiClient
        {
            private readonly Func<LightningInvoiceResponse> _createInvoice;
            private readonly Func<string, LightningPaymentStatusResponse> _getStatus;

            public FakeLightningApiClient(Func<LightningInvoiceResponse> createInvoice, Func<string, LightningPaymentStatusResponse> getStatus)
            {
                _createInvoice = createInvoice;
                _getStatus = getStatus;
            }

            public Task<LightningInvoiceResponse> CreateInvoiceAsync(int amountSats, string description, CancellationToken cancellationToken = default)
                => Task.FromResult(_createInvoice());

            public Task<LightningPaymentStatusResponse> GetPaymentStatusAsync(string paymentHash, CancellationToken cancellationToken = default)
                => Task.FromResult(_getStatus(paymentHash));
        }
    }
}
