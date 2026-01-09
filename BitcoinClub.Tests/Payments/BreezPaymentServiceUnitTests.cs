using System;
using System.Threading;
using System.Threading.Tasks;
using BitcoinClub.Data;
using BitcoinClub.Infrastructure.Payments;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BitcoinClub.Tests.Payments
{
    public class BreezPaymentServiceUnitTests
    {
        [Fact]
        public async Task InitiateMembershipPaymentAsync_ValidInput_CreatesSubscription_AndReturnsInvoiceData()
        {
            var breez = new FakeBreezApiClient(
                createInvoice: () => new BreezPaymentInitResponse("p1", "bolt11"),
                getStatus: _ => new BreezPaymentStatusResponse(false, null));

            var db = CreateInMemoryDb();
            var sut = new BreezPaymentService(breez, db);

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
            var breez = new FakeBreezApiClient(
                createInvoice: () => new BreezPaymentInitResponse("p1", "bolt11"),
                getStatus: _ => new BreezPaymentStatusResponse(false, null));

            var db = CreateInMemoryDb();
            var sut = new BreezPaymentService(breez, db);

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

        private sealed class FakeBreezApiClient : IBreezApiClient
        {
            private readonly Func<BreezPaymentInitResponse> _createInvoice;
            private readonly Func<string, BreezPaymentStatusResponse> _getStatus;

            public FakeBreezApiClient(Func<BreezPaymentInitResponse> createInvoice, Func<string, BreezPaymentStatusResponse> getStatus)
            {
                _createInvoice = createInvoice;
                _getStatus = getStatus;
            }

            public Task<BreezPaymentInitResponse> CreateInvoiceAsync(int amountSats, string description, CancellationToken cancellationToken = default)
                => Task.FromResult(_createInvoice());

            public Task<BreezPaymentStatusResponse> GetPaymentStatusAsync(string paymentId, CancellationToken cancellationToken = default)
                => Task.FromResult(_getStatus(paymentId));
        }
    }
}
