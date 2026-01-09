using System;
using System.Threading.Tasks;
using BitcoinClub.Infrastructure.Auth.Providers;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace BitcoinClub.Tests.AuthProviders
{
    public class MockIdentityAuthProviderTests
    {
        [Fact]
        public async Task EmailPasswordAuthProvider_InvokesPasswordSignInAsync_WithExpectedArgs()
        {
            var userStore = new Mock<IUserStore<IdentityUser>>();
            var options = new Mock<Microsoft.Extensions.Options.IOptions<IdentityOptions>>();
            options.Setup(o => o.Value).Returns(new IdentityOptions());

            var userManager = new UserManager<IdentityUser>(
                userStore.Object,
                options.Object,
                new Mock<IPasswordHasher<IdentityUser>>().Object,
                Array.Empty<IUserValidator<IdentityUser>>(),
                Array.Empty<IPasswordValidator<IdentityUser>>(),
                new Mock<ILookupNormalizer>().Object,
                new IdentityErrorDescriber(),
                new Mock<IServiceProvider>().Object,
                new Mock<Microsoft.Extensions.Logging.ILogger<UserManager<IdentityUser>>>().Object);

            var signInManager = new Mock<SignInManager<IdentityUser>>(
                userManager,
                Mock.Of<Microsoft.AspNetCore.Http.IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<IdentityUser>>(),
                Microsoft.Extensions.Options.Options.Create(new IdentityOptions()),
                Mock.Of<Microsoft.Extensions.Logging.ILogger<SignInManager<IdentityUser>>>(),
                Mock.Of<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>(),
                Mock.Of<IUserConfirmation<IdentityUser>>()
            );

            signInManager
                .Setup(s => s.PasswordSignInAsync("a@b.com", "pw", false, true))
                .ReturnsAsync(SignInResult.Failed);

            var provider = new EmailPasswordAuthProvider(signInManager.Object);
            await provider.SignInAsync(new AuthSignInRequest("a@b.com", "pw", false));

            signInManager.Verify(s => s.PasswordSignInAsync("a@b.com", "pw", false, true), Times.Once);
        }
    }
}
