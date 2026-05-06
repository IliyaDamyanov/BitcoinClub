using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BitcoinClub.Controllers;
using BitcoinClub.Data;
using BitcoinClub.Models;
using BitcoinClub.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BitcoinClub.Tests.Membership
{
    public class MembershipControllerTests
    {
        [Fact]
        public async Task Index_WhenSubscriptionExists_ReturnsViewModel()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
                .Options;

            await using var db = new ApplicationDbContext(options);

            var userId = "u1";
            var expectedExpiration = DateTime.UtcNow.AddDays(30);

            db.Subscriptions.Add(new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpirationDate = expectedExpiration,
                LastPaymentDate = null
            });
            await db.SaveChangesAsync();

            var controller = new MembershipController(db);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId)
                    }, "Test"))
                }
            };

            var result = await controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<MembershipDashboardViewModel>(view.Model);
            Assert.Equal(expectedExpiration, vm.ExpirationDate);
        }

        [Fact]
        public async Task Index_ShowsOnlyCurrentUsersPaymentHistory_WithProviderAndPaidDates()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
                .Options;

            await using var db = new ApplicationDbContext(options);

            var userId = "u1";
            var subscriptionId = Guid.NewGuid();
            db.Subscriptions.Add(new Subscription
            {
                Id = subscriptionId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                ExpirationDate = DateTime.UtcNow.AddDays(20),
                LastPaymentDate = DateTime.UtcNow.AddDays(-1)
            });
            db.Payments.Add(new Payment
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SubscriptionId = subscriptionId,
                Provider = "glow-pay",
                ProviderPaymentId = "pay-current",
                AmountSats = 2100,
                Status = "paid",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                PaidAt = DateTime.UtcNow
            });
            db.Payments.Add(new Payment
            {
                Id = Guid.NewGuid(),
                UserId = "other-user",
                SubscriptionId = Guid.NewGuid(),
                Provider = "glow-pay",
                ProviderPaymentId = "pay-other",
                AmountSats = 9999,
                Status = "paid",
                CreatedAt = DateTime.UtcNow,
                PaidAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();

            var controller = new MembershipController(db);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId)
                    }, "Test"))
                }
            };

            var result = await controller.Index();

            var view = Assert.IsType<ViewResult>(result);
            var vm = Assert.IsType<MembershipDashboardViewModel>(view.Model);
            var payment = Assert.Single(vm.PaymentHistory);
            Assert.Equal("glow-pay", payment.Provider);
            Assert.Equal(2100, payment.AmountSats);
            Assert.Equal("paid", payment.Status);
            Assert.NotNull(payment.PaidAt);
            Assert.True(vm.RemainingDays > 0);
        }

        [Fact]
        public async Task Index_WhenNoSubscription_ReturnsNoSubscriptionView()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
                .Options;

            await using var db = new ApplicationDbContext(options);

            var controller = new MembershipController(db);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "u1")
                    }, "Test"))
                }
            };

            var result = await controller.Index();
            var view = Assert.IsType<ViewResult>(result);
            Assert.Equal("NoSubscription", view.ViewName);
        }
    }
}
