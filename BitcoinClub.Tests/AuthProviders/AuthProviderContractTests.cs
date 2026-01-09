using System;
using System.Threading.Tasks;
using BitcoinClub.Infrastructure.Auth.Providers;
using Xunit;

namespace BitcoinClub.Tests.AuthProviders
{
    public class AuthProviderContractTests
    {
        private sealed class FakeAuthProvider : IAuthProvider
        {
            public string Name => "fake";

            public Task<AuthSignInResult> SignInAsync(AuthSignInRequest request, System.Threading.CancellationToken cancellationToken = default)
                => Task.FromResult(new AuthSignInResult(true, false, false, null));

            public Task<AuthSignOutResult> SignOutAsync(System.Threading.CancellationToken cancellationToken = default)
                => Task.FromResult(new AuthSignOutResult(true, null));
        }

        [Fact]
        public async Task SignInAsync_ReturnsResult_WithName()
        {
            IAuthProvider provider = new FakeAuthProvider();

            Assert.False(string.IsNullOrWhiteSpace(provider.Name));

            var result = await provider.SignInAsync(new AuthSignInRequest("a@b.com", "pw"));
            Assert.NotNull(result);
        }

        [Fact]
        public async Task SignOutAsync_ReturnsResult()
        {
            IAuthProvider provider = new FakeAuthProvider();

            var result = await provider.SignOutAsync();
            Assert.True(result.Succeeded);
        }
    }
}
