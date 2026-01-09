using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BitcoinClub.Tests.Landing
{
    public class LandingPageViewRenderingTests
    {
        [Fact]
        public async void HomePage_RendersLandingContent()
        {
            using var factory = new WebApplicationFactory<BitcoinClub.Program>();
            using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var resp = await client.GetAsync("/");
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

            var html = await resp.Content.ReadAsStringAsync();
            Assert.Contains("Bitcoin Club", html);
            Assert.Contains("Mission", html);
            Assert.Contains("Contact", html);
        }
    }
}
