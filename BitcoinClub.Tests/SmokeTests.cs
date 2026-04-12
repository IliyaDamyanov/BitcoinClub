using BitcoinClub.Controllers;

using BitcoinClub.Services.Landing;
using BitcoinClub.Tests.TestDoubles;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace BitcoinClub.Tests;

public class SmokeTests
{
    [Fact]
    public void TestAssembly_Loads()
    {
        Assert.True(true);
    }

    [Fact]
    public void HomeController_CanBeInstantiated()
    {
        var localizer = new StubStringLocalizer<LandingPageStrings>(new Dictionary<string, string>
        {
            ["ClubName"] = "Club"
        });

        var controller = new HomeController(
            NullLogger<HomeController>.Instance,
            new LandingPageContentService(localizer),
            new StubEventsService());

        Assert.NotNull(controller);
    }
}
