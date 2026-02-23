using BitcoinClub.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace BitcoinClub.Tests.Auth;

public sealed class AccountControllerRegisterTests
{
    [Fact]
    public async Task Register_Post_WhenModelStateInvalid_ReturnsViewWithSameModel()
    {
        var userStore = new Mock<IUserStore<IdentityUser>>();
        var userManager = MockUserManager(userStore.Object);
        var signInManager = MockSignInManager(userManager.Object);
        var emailSender = new Mock<IEmailSender>();

        var logger = new Mock<ILogger<AccountController>>();
        var sut = new AccountController(userManager.Object, userStore.Object, signInManager.Object, emailSender.Object, logger.Object);
        sut.ModelState.AddModelError("Email", "Required");

        var model = new AccountController.RegisterViewModel
        {
            Email = "",
            Password = "",
            ConfirmPassword = ""
        };

        var result = await sut.Register(model);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(model, view.Model);
    }

    private static Mock<UserManager<IdentityUser>> MockUserManager(IUserStore<IdentityUser> store)
    {
        var identityOptions = Options.Create(new IdentityOptions
        {
            SignIn = new SignInOptions { RequireConfirmedAccount = true }
        });

        var mgr = new Mock<UserManager<IdentityUser>>(
            store,
            identityOptions,
            null,
            Array.Empty<IUserValidator<IdentityUser>>(),
            Array.Empty<IPasswordValidator<IdentityUser>>(),
            null,
            null,
            null,
            null);

        mgr.SetupGet(m => m.SupportsUserEmail).Returns(true);

        mgr.Setup(m => m.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());

        return mgr;
    }

    private static Mock<SignInManager<IdentityUser>> MockSignInManager(UserManager<IdentityUser> userManager)
    {
        var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();

        return new Mock<SignInManager<IdentityUser>>(
            userManager,
            contextAccessor.Object,
            claimsFactory.Object,
            null,
            null,
            null,
            null);
    }
}
