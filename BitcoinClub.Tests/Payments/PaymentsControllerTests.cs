using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using BitcoinClub.Controllers;
using BitcoinClub.Data;
using BitcoinClub.Infrastructure.Payments;
using BitcoinClub.Models;
using BitcoinClub.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace BitcoinClub.Tests.Payments
{
    public class PaymentsControllerTests
    {
        [Fact]
        public async Task Initiate_Post_CreatesPaymentRecord_AndReturnsViewWithInvoice()
        {
            var userId = "user-1";
            var db = CreateInMemoryDb();
            db.Subscriptions.Add(new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow
            });
            await db.SaveChangesAsync();

            var payments = new FakePaymentService();
            var controller = new PaymentsController(db, payments, NullLogger<PaymentsController>.Instance);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = CreateHttpContext(userId)
            };

            var vm = new PaymentInitiateViewModel { AmountSats = 1234, Description = "Membership" };

            var result = await controller.Initiate(vm);

            var view = Assert.IsType<ViewResult>(result);
            var outVm = Assert.IsType<PaymentInitiateViewModel>(view.Model);

            Assert.Equal("prov-1", outVm.PaymentId);
            Assert.Equal("bolt11", outVm.PaymentRequest);
            Assert.Equal("https://glow-pay.co/pay/prov-1", outVm.PaymentUrl);
            Assert.NotNull(outVm.PaymentRecordId);

            var paymentRow = await db.Payments.SingleAsync(p => p.UserId == userId);
            Assert.Equal("glow-pay", paymentRow.Provider);
            Assert.Equal("prov-1", paymentRow.ProviderPaymentId);
            Assert.Equal("initiated", paymentRow.Status);
        }

        private static ApplicationDbContext CreateInMemoryDb()
        {
            var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
                .Options;

            return new ApplicationDbContext(opts);
        }

        private static DefaultHttpContext CreateHttpContext(string userId)
        {
            var ctx = new DefaultHttpContext();
            ctx.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "Test"));
            return ctx;
        }

        private sealed class FakePaymentService : IPaymentService
        {
            public Task<PaymentInitiationResult> InitiateMembershipPaymentAsync(string userId, int amountSats, string description, CancellationToken cancellationToken = default)
                => Task.FromResult(new PaymentInitiationResult(Guid.NewGuid(), "prov-1", "bolt11", amountSats, "https://glow-pay.co/pay/prov-1"));

            public Task<PaymentVerificationResult> VerifyPaymentAsync(Guid subscriptionId, string paymentId, CancellationToken cancellationToken = default)
                => Task.FromResult(new PaymentVerificationResult(false, null, null));
        }
    }
}
