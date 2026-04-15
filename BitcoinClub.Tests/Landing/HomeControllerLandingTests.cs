using BitcoinClub.Controllers;

using BitcoinClub.Services.Landing;
using BitcoinClub.Tests.Services.CalendarEvents.Helpers;
using BitcoinClub.Tests.TestDoubles;
using BitcoinClub.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace BitcoinClub.Tests.Landing
{
    public class HomeControllerLandingTests
    {
        [Fact]
        public async Task Index_ReturnsView_WithLandingPageViewModel()
        {
            var localizer = new StubStringLocalizer<LandingPageStrings>(new Dictionary<string, string>
            {
                ["ClubName"] = "Club"
            });

            var controller = new HomeController(
                NullLogger<HomeController>.Instance,
                new LandingPageContentService(localizer),
                new StubCalendarEventsService());

            var result = await controller.Index(null);

            var view = Assert.IsType<ViewResult>(result);
            Assert.IsType<LandingPageViewModel>(view.Model);
        }
    }
}
