using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BitcoinClub.Tests.Admin
{
    public class AdminRoutingIntegrationTests
    {
        [Fact]
        public async void AdminHome_DefaultRoute_Works()
        {
            using var factory = new WebApplicationFactory<BitcoinClub.Program>();
            using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

            var resp = await client.GetAsync("/Admin");
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }
    }
}
