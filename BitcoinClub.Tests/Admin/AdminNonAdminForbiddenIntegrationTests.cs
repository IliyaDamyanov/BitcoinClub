using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Xunit;

namespace BitcoinClub.Tests.Admin
{
    public class AdminNonAdminForbiddenIntegrationTests
    {
        [Fact]
        public async Task AuthenticatedNonAdmin_IsForbidden()
        {
            using var factory = new WebApplicationFactory<BitcoinClub.Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddAuthentication("Test")
                            .AddScheme<AuthenticationSchemeOptions, NonAdminAuthHandler>("Test", _ => { });

                        services.PostConfigureAll<AuthenticationOptions>(o =>
                        {
                            o.DefaultAuthenticateScheme = "Test";
                            o.DefaultChallengeScheme = "Test";
                        });
                    });
                });

            using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");

            var resp = await client.GetAsync("/Admin");
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        private sealed class NonAdminAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            public NonAdminAuthHandler(
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
                    new Claim(ClaimTypes.NameIdentifier, "user-1"),
                    new Claim(ClaimTypes.Name, "user-1"),
                    // no admin role claim
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
        }
    }
}
