using System.IO;
using Xunit;

namespace BitcoinClub.Tests.Views;

public class NavbarThemeTests
{
    [Fact]
    public void Layout_DoesNotRenderGlobalNavbar()
    {
        var layoutPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "BitcoinClub",
            "Views",
            "Shared",
            "_Layout.cshtml"));

        var layout = File.ReadAllText(layoutPath);

        Assert.DoesNotContain("<nav", layout);
        Assert.DoesNotContain("navbar", layout);
    }

    [Fact]
    public void LandingPage_RendersAuthLinksNearLanguageButton()
    {
        var viewPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "BitcoinClub",
            "Views",
            "Home",
            "Index.cshtml"));

        var view = File.ReadAllText(viewPath);

        Assert.Contains("bc-lang-button", view);
        Assert.Contains("<partial name=\"_LoginPartial\"", view);
    }
}
