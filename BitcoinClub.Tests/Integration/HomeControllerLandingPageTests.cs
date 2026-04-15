
using BitcoinClub.Services.Landing;
using BitcoinClub.Tests.Services.CalendarEvents.Helpers;
using BitcoinClub.Tests.TestDoubles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace BitcoinClub.Tests.Integration;

public sealed class HomeControllerLandingPageTests
{
    [Fact]
    public async Task Index_WhenLangIsEn_RedirectsToCleanUrl()
    {
        var localizer = new StubStringLocalizer<LandingPageStrings>(new Dictionary<string, string>
        {
            ["ClubName"] = "Club"
        });

        var controller = new BitcoinClub.Controllers.HomeController(
            NullLogger<BitcoinClub.Controllers.HomeController>.Instance,
            new LandingPageContentService(localizer),
            new StubCalendarEventsService());

        // Switching language now persists via cookie + redirect so that the middleware
        // sets CultureInfo.CurrentUICulture for the whole pipeline on the next request.
        var result = await controller.Index("EN");

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(BitcoinClub.Controllers.HomeController.Index), redirect.ActionName);
    }
}
