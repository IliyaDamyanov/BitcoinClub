using System.IO;
using Xunit;

namespace BitcoinClub.Tests.StaticAssets;

public class SiteCssThemeTests
{
    [Fact]
    public void SiteCss_SetsBlackBackgroundOnHtmlAndBody()
    {
        var cssPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "BitcoinClub",
            "wwwroot",
            "css",
            "site.css"));

        var css = File.ReadAllText(cssPath);

        Assert.Contains("html, body", css);
        Assert.Contains("background-color: #000", css);
    }

    [Fact]
    public void SiteCss_SetsDarkBackgroundOnMainContainer()
    {
        var cssPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "BitcoinClub",
            "wwwroot",
            "css",
            "site.css"));

        var css = File.ReadAllText(cssPath);

        Assert.Contains("body > .container", css);
        Assert.Contains("background-color: #212529", css);
    }
}
