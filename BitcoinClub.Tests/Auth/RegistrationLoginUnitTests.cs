using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace BitcoinClub.Tests.Auth
{
    public class RegistrationLoginUnitTests
    {
        [Fact]
        public async Task Register_CallsCreateAsync_WithEmailAsUserName()
        {
            var userStore = new Mock<IUserStore<IdentityUser>>();
            var mgr = MockUserManager(userStore.Object);

            mgr.Setup(m => m.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var email = "a@b.com";
            var password = "P@ssw0rd!";

            var user = new IdentityUser { UserName = email, Email = email };
            var result = await mgr.Object.CreateAsync(user, password);

            Assert.True(result.Succeeded);
            mgr.Verify(m => m.CreateAsync(It.Is<IdentityUser>(u => u.Email == email && u.UserName == email), password), Times.Once);
        }

        [Fact]
        public async Task Login_CallsPasswordSignInAsync()
        {
            var signInManager = MockSignInManager();
            signInManager.Setup(s => s.PasswordSignInAsync("a@b.com", "P@ssw0rd!", false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var result = await signInManager.Object.PasswordSignInAsync("a@b.com", "P@ssw0rd!", false, false);

            Assert.True(result.Succeeded);
            signInManager.Verify(s => s.PasswordSignInAsync("a@b.com", "P@ssw0rd!", false, false), Times.Once);
        }

        private static Mock<UserManager<IdentityUser>> MockUserManager(IUserStore<IdentityUser> store)
        {
            var options = new Mock<Microsoft.Extensions.Options.IOptions<IdentityOptions>>();
            options.Setup(o => o.Value).Returns(new IdentityOptions());

            return new Mock<UserManager<IdentityUser>>(
                store,
                options.Object,
                new Mock<IPasswordHasher<IdentityUser>>().Object,
                Array.Empty<IUserValidator<IdentityUser>>(),
                Array.Empty<IPasswordValidator<IdentityUser>>(),
                new Mock<ILookupNormalizer>().Object,
                new IdentityErrorDescriber(),
                new Mock<IServiceProvider>().Object,
                new Mock<Microsoft.Extensions.Logging.ILogger<UserManager<IdentityUser>>>().Object);
        }

        private static Mock<SignInManager<IdentityUser>> MockSignInManager()
        {
            var userStore = new Mock<IUserStore<IdentityUser>>();
            var userManager = MockUserManager(userStore.Object);

            var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
            var options = new Mock<Microsoft.Extensions.Options.IOptions<IdentityOptions>>();
            options.Setup(o => o.Value).Returns(new IdentityOptions());

            return new Mock<SignInManager<IdentityUser>>(
                userManager.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                options.Object,
                new Mock<Microsoft.Extensions.Logging.ILogger<SignInManager<IdentityUser>>>().Object,
                new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<IdentityUser>>().Object);
        }
    }
}
