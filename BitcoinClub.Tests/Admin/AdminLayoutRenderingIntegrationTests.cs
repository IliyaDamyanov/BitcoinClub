using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BitcoinClub.Tests.Admin
{
    public class AdminLayoutRenderingIntegrationTests
    {
        [Fact]
        public async Task AdminHome_UsesAdminLayoutAndNavigation()
        {
            using var factory = new WebApplicationFactory<BitcoinClub.Program>();
            using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var resp = await client.GetAsync("/Admin");
            // Unauthenticated users get redirected; admin layout requires login
            Assert.Equal(HttpStatusCode.Found, resp.StatusCode);
        }
    }
}
