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
        Assert.Contains("background-color: #050508", css);
    }

    [Fact]
    public void SiteCss_SetsInterFontFamily()
    {
        var cssPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..",
            "BitcoinClub",
            "wwwroot",
            "css",
            "site.css"));

        var css = File.ReadAllText(cssPath);

        Assert.Contains("Inter", css);
        Assert.Contains("font-family", css);
    }
}
