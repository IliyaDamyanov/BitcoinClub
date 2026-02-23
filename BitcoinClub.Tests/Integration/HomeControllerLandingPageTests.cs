
using BitcoinClub.Services.Landing;
using BitcoinClub.Tests.TestDoubles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace BitcoinClub.Tests.Integration;

public sealed class HomeControllerLandingPageTests
{
    [Fact]
    public void Index_WhenLangIsEn_ReturnsViewWithEnModel()
    {
        var localizer = new StubStringLocalizer<LandingPageStrings>(new Dictionary<string, string>
        {
            ["ClubName"] = "Club"
        });

        var controller = new BitcoinClub.Controllers.HomeController(
            NullLogger<BitcoinClub.Controllers.HomeController>.Instance,
            new LandingPageContentService(localizer));

        var result = controller.Index("EN");

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<BitcoinClub.ViewModels.LandingPageViewModel>(viewResult.Model);
        Assert.Equal("EN", model.Lang);
    }
}
