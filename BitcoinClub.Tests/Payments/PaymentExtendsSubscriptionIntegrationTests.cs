using System;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BitcoinClub.Data;
using BitcoinClub.Infrastructure.Payments;
using BitcoinClub.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace BitcoinClub.Tests.Payments
{
    public class PaymentExtendsSubscriptionIntegrationTests
    {
        [Fact]
        public async Task VerifyEndpoint_WhenPaid_ExtendsSubscription()
        {
            var cs = Environment.GetEnvironmentVariable("BITCOINCLUB_TEST_PG_CONNECTION");
            if (string.IsNullOrWhiteSpace(cs))
            {
                return;
            }

            var paidAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

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

                        // Force Breez API to return paid.
                        services.RemoveAll(typeof(IBreezApiClient));
                        services.AddSingleton<IBreezApiClient>(new PaidBreezApiClient(paidAt));
                    });
                });

            using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

            var userId = Guid.NewGuid().ToString("N");
            var subscriptionId = Guid.NewGuid();
            var oldExpiration = DateTime.UtcNow.AddDays(-1);

            Guid paymentRecordId;

            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await db.Database.MigrateAsync();

                db.Users.Add(new Microsoft.AspNetCore.Identity.IdentityUser { Id = userId, UserName = $"u{userId}@test.local", Email = $"u{userId}@test.local" });
                db.Subscriptions.Add(new Subscription
                {
                    Id = subscriptionId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    ExpirationDate = oldExpiration
                });

                var payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    SubscriptionId = subscriptionId,
                    Provider = "breez",
                    ProviderPaymentId = "prov-1",
                    AmountSats = 1000,
                    PaymentRequest = "bolt11",
                    Status = "initiated",
                    CreatedAt = DateTime.UtcNow
                };

                db.Payments.Add(payment);
                await db.SaveChangesAsync();

                paymentRecordId = payment.Id;
            }

            TestAuthHandler.UserId = userId;

            var form = new FormUrlEncodedContent(new[]
            {
                new System.Collections.Generic.KeyValuePair<string, string>("subscriptionId", subscriptionId.ToString()),
                new System.Collections.Generic.KeyValuePair<string, string>("paymentRecordId", paymentRecordId.ToString())
            });

            var resp = await client.PostAsync("/Payments/Verify", form);
            Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);

            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var sub = await db.Subscriptions.SingleAsync(s => s.Id == subscriptionId);
                Assert.True(sub.ExpirationDate > oldExpiration);

                var pay = await db.Payments.SingleAsync(p => p.Id == paymentRecordId);
                Assert.Equal("paid", pay.Status);
            }
        }

        private sealed class PaidBreezApiClient : IBreezApiClient
        {
            private readonly long _paidAt;

            public PaidBreezApiClient(long paidAt)
            {
                _paidAt = paidAt;
            }

            public Task<BreezPaymentInitResponse> CreateInvoiceAsync(int amountSats, string description, System.Threading.CancellationToken cancellationToken = default)
                => Task.FromResult(new BreezPaymentInitResponse("prov-1", "bolt11"));

            public Task<BreezPaymentStatusResponse> GetPaymentStatusAsync(string paymentId, System.Threading.CancellationToken cancellationToken = default)
                => Task.FromResult(new BreezPaymentStatusResponse(IsPaid: true, PaidAtUnixSeconds: _paidAt));
        }

        private sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            public static string? UserId { get; set; }

            public TestAuthHandler(
                IOptionsMonitor<AuthenticationSchemeOptions> options,
                ILoggerFactory logger,
                UrlEncoder encoder)
                : base(options, logger, encoder)
            {
            }

            protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, UserId ?? "test-user")
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
        }
    }
}
