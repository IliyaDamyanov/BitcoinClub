using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BitcoinClub.Tests.Auth
{
    public class RegisterLoginProtectedFlowIntegrationTests : IClassFixture<WebApplicationFactory<BitcoinClub.Program>>
    {
        private readonly WebApplicationFactory<BitcoinClub.Program> _factory;

        public RegisterLoginProtectedFlowIntegrationTests(WebApplicationFactory<BitcoinClub.Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Anonymous_User_IsRedirectedToLogin_ForProtectedMembersPage()
        {
            using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var resp = await client.GetAsync("/Members");

            Assert.Equal(HttpStatusCode.Redirect, resp.StatusCode);
            Assert.NotNull(resp.Headers.Location);
            Assert.Contains("/Identity/Account/Login", resp.Headers.Location!.ToString());
        }
    }
}
