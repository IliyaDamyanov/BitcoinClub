using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BitcoinClub.Tests.Admin
{
    public class AdminLayoutRenderingIntegrationTests
    {
        [Fact]
        public async void AdminHome_UsesAdminLayoutAndNavigation()
        {
            using var factory = new WebApplicationFactory<BitcoinClub.Program>();
            using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var resp = await client.GetAsync("/Admin");
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

            var html = await resp.Content.ReadAsStringAsync();

            Assert.Contains("Admin - BitcoinClub", html);
            Assert.Contains("Dashboard", html);
            Assert.Contains("Back to site", html);
        }
    }
}
