using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BitcoinClub.Tests.Landing
{
    public class LandingPageViewRenderingTests
    {
        [Fact]
        public async Task HomePage_RendersLandingContent()
        {
            using var factory = new WebApplicationFactory<BitcoinClub.Program>();
            // Allow auto-redirect: /?lang=EN now sets the cookie and redirects to /
            using var client = factory.CreateClient();

            var resp = await client.GetAsync("/?lang=EN");
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

            var html = await resp.Content.ReadAsStringAsync();
            Assert.Contains("Bitcoin Club", html);
            Assert.Contains("Mission", html);
            Assert.Contains("Events", html);
            Assert.Contains("Contact", html);
        }
    }
}
