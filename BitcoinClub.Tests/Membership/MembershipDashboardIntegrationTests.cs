using System;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BitcoinClub.Data;
using BitcoinClub.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace BitcoinClub.Tests.Membership
{
    public class MembershipDashboardIntegrationTests
    {
        [Fact]
        public async Task AuthenticatedUser_DashboardLoadsAndShowsSubscriptionDates_WhenConfigured()
        {
            var cs = Environment.GetEnvironmentVariable("BITCOINCLUB_TEST_PG_CONNECTION");
            if (string.IsNullOrWhiteSpace(cs))
            {
                return;
            }

            using var factory = new WebApplicationFactory<BitcoinClub.Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddAuthentication("Test")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

                        services.PostConfigureAll<AuthenticationOptions>(o =>
                        {
                            o.DefaultAuthenticateScheme = "Test";
                            o.DefaultChallengeScheme = "Test";
                        });

                        services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
                        services.RemoveAll(typeof(ApplicationDbContext));

                        services.AddDbContext<ApplicationDbContext>(opts => opts.UseNpgsql(cs));
                    });
                });

            using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

            var userId = Guid.NewGuid().ToString("N");
            var expiration = DateTime.UtcNow.AddDays(7);
            var lastPayment = DateTime.UtcNow.AddDays(-1);

            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await db.Database.MigrateAsync();

                db.Users.Add(new Microsoft.AspNetCore.Identity.IdentityUser { Id = userId, UserName = $"u{userId}@test.local", Email = $"u{userId}@test.local" });
                db.Subscriptions.Add(new Subscription
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    ExpirationDate = expiration,
                    LastPaymentDate = lastPayment
                });

                await db.SaveChangesAsync();
            }

            TestAuthHandler.UserId = userId;

            var resp = await client.GetAsync("/Membership");
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

            var html = await resp.Content.ReadAsStringAsync();
            Assert.Contains(expiration.ToString("u"), html);
            Assert.Contains(lastPayment.ToString("u"), html);
        }

        private sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            public static string UserId { get; set; } = string.Empty;

            public TestAuthHandler(
                IOptionsMonitor<AuthenticationSchemeOptions> options,
                ILoggerFactory logger,
                UrlEncoder encoder)
                : base(options, logger, encoder)
            {
            }

            protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, UserId),
                    new Claim(ClaimTypes.Name, "test")
                }, Scheme.Name);

                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
        }
    }
}
