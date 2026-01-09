using System;
using System.Threading.Tasks;
using BitcoinClub.Infrastructure.Auth.Providers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace BitcoinClub.Tests.AuthProviders
{
    public class EmailPasswordAuthProviderTests
    {
        [Fact]
        public async Task SignInAsync_NullRequest_Throws()
        {
            var provider = new EmailPasswordAuthProvider(MockSignInManager().Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => provider.SignInAsync(null!));
        }

        [Fact]
        public async Task SignInAsync_MapsIdentityResult()
        {
            var signInManager = MockSignInManager();
            signInManager
                .Setup(s => s.PasswordSignInAsync("a@b.com", "pw", true, true))
                .ReturnsAsync(SignInResult.Success);

            var provider = new EmailPasswordAuthProvider(signInManager.Object);

            var result = await provider.SignInAsync(new AuthSignInRequest("a@b.com", "pw", true));

            Assert.True(result.Succeeded);
            Assert.Null(result.Error);
        }

        [Fact]
        public async Task SignOutAsync_CallsUnderlyingManager()
        {
            var signInManager = MockSignInManager();
            signInManager.Setup(s => s.SignOutAsync()).Returns(Task.CompletedTask);

            var provider = new EmailPasswordAuthProvider(signInManager.Object);

            var result = await provider.SignOutAsync();

            Assert.True(result.Succeeded);
            signInManager.Verify(s => s.SignOutAsync(), Times.Once);
        }

        private static Mock<SignInManager<IdentityUser>> MockSignInManager()
        {
            var userStore = new Mock<IUserStore<IdentityUser>>();
            var options = new Mock<Microsoft.Extensions.Options.IOptions<IdentityOptions>>();
            options.Setup(o => o.Value).Returns(new IdentityOptions());

            var userManager = new Mock<UserManager<IdentityUser>>(
                userStore.Object,
                options.Object,
                new Mock<IPasswordHasher<IdentityUser>>().Object,
                Array.Empty<IUserValidator<IdentityUser>>(),
                Array.Empty<IPasswordValidator<IdentityUser>>(),
                new Mock<ILookupNormalizer>().Object,
                new IdentityErrorDescriber(),
                new Mock<IServiceProvider>().Object,
                new Mock<Microsoft.Extensions.Logging.ILogger<UserManager<IdentityUser>>>().Object);

            var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
            var identityOptions = Options.Create(new IdentityOptions());

            return new Mock<SignInManager<IdentityUser>>(
                userManager.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                identityOptions,
                new Mock<Microsoft.Extensions.Logging.ILogger<SignInManager<IdentityUser>>>().Object,
                new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<IdentityUser>>().Object);
        }
    }
}
