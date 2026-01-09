using System;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BitcoinClub.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace BitcoinClub.Tests.Admin
{
    public class PostCreateUiIntegrationTests
    {
        [Fact]
        public async Task CreatePost_UiFlow_PersistsRow()
        {
            var cs = Environment.GetEnvironmentVariable("BITCOINCLUB_TEST_PG_CONNECTION");
            if (string.IsNullOrWhiteSpace(cs))
            {
                return;
            }

            var adminUserId = Guid.NewGuid().ToString("N");

            using var factory = new WebApplicationFactory<BitcoinClub.Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddAuthentication("Test")
                            .AddScheme<AuthenticationSchemeOptions, AdminAuthHandler>("Test", _ => { });

                        services.PostConfigureAll<AuthenticationOptions>(o =>
                        {
                            o.DefaultAuthenticateScheme = "Test";
                            o.DefaultChallengeScheme = "Test";
                        });

                        services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
                        services.RemoveAll(typeof(ApplicationDbContext));
                        services.AddDbContext<ApplicationDbContext>(opts => opts.UseNpgsql(cs));

                        AdminAuthHandler.UserId = adminUserId;
                    });
                });

            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await db.Database.MigrateAsync();
                db.Users.Add(new Microsoft.AspNetCore.Identity.IdentityUser { Id = adminUserId, UserName = $"a{adminUserId}@test.local", Email = $"a{adminUserId}@test.local" });
                await db.SaveChangesAsync();
            }

            using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

            // Get antiforgery token.
            var getResp = await client.GetAsync("/Admin/Posts/Create");
            Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);
            var html = await getResp.Content.ReadAsStringAsync();

            var token = ExtractAntiForgeryToken(html);
            Assert.False(string.IsNullOrWhiteSpace(token));

            var form = new MultipartFormDataContent();
            form.Add(new StringContent(token), "__RequestVerificationToken");
            form.Add(new StringContent("hello from ui"), "TextContent");
            form.Add(new StringContent("nostr"), "SelectedPlatforms");

            var postResp = await client.PostAsync("/Admin/Posts/Create", form);
            Assert.Equal(HttpStatusCode.Redirect, postResp.StatusCode);

            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var row = await db.Posts.OrderByDescending(p => p.CreatedAt).FirstAsync(p => p.AdminUserId == adminUserId);
                Assert.Equal("hello from ui", row.TextContent);
                Assert.Contains("nostr", row.Platforms);
            }
        }

        private static string ExtractAntiForgeryToken(string html)
        {
            // Matches: <input name="__RequestVerificationToken" type="hidden" value="..." />
            const string marker = "name=\"__RequestVerificationToken\"";
            var idx = html.IndexOf(marker, StringComparison.Ordinal);
            if (idx < 0) return string.Empty;

            var valueMarker = "value=\"";
            var valueIdx = html.IndexOf(valueMarker, idx, StringComparison.Ordinal);
            if (valueIdx < 0) return string.Empty;
            valueIdx += valueMarker.Length;

            var endIdx = html.IndexOf('"', valueIdx);
            if (endIdx < 0) return string.Empty;

            return html.Substring(valueIdx, endIdx - valueIdx);
        }

        private sealed class AdminAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            public static string? UserId { get; set; }

            public AdminAuthHandler(
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
                    new Claim(ClaimTypes.NameIdentifier, UserId ?? "admin-1"),
                    new Claim(ClaimTypes.Role, "admin")
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
        }
    }
}
