using BitcoinClub.Controllers;
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
        var controller = new HomeController(NullLogger<HomeController>.Instance);
        Assert.NotNull(controller);
    }
}
