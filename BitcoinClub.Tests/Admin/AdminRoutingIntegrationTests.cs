using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BitcoinClub.Tests.Admin
{
    public class AdminRoutingIntegrationTests
    {
        [Fact]
        public async Task AdminHome_DefaultRoute_Works()
        {
            using var factory = new WebApplicationFactory<BitcoinClub.Program>();
            using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var resp = await client.GetAsync("/Admin");
            // Unauthenticated users are redirected to login
            Assert.Equal(HttpStatusCode.Found, resp.StatusCode);
            Assert.Contains("/Account/Login", resp.Headers.Location?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        }
    }
}
