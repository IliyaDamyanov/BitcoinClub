using System.Globalization;
using BitcoinClub.Resources;
using BitcoinClub.Services.Landing;
using Microsoft.Extensions.Localization;
using Moq;
using Xunit;

namespace BitcoinClub.Tests.Services.Landing;

public sealed class LandingPageContentServiceTests
{
    private static LandingPageContentService CreateSut(Dictionary<string, string> values)
    {
        var localizer = new Mock<IStringLocalizer<LandingPageStrings>>();

        localizer
            .Setup(l => l[It.IsAny<string>()])
            .Returns((string key) =>
            {
                values.TryGetValue(key, out var value);
                value ??= "__missing__";
                return new LocalizedString(key, value, resourceNotFound: value == "__missing__");
            });

        return new LandingPageContentService(localizer.Object);
    }

    [Fact]
    public void Get_WhenLangIsNull_ReturnsBgAndShowsEnToggle()
    {
        var sut = CreateSut(new()
        {
            ["ClubName"] = "Клуб",
            ["Goals_1"] = "Цел 1",
            ["Means_1"] = "Средство 1",
            ["MembershipDetails"] = "М",
            ["SupportDetails"] = "П",
        });

        var vm = sut.Get(null);

        Assert.Equal("BG", vm.Lang);
        Assert.Equal("EN", vm.ChangeLanguageButtonText);
        Assert.Equal("Клуб", vm.ClubName);
        Assert.Contains("Цел 1", vm.Goals);
    }

    [Theory]
    [InlineData("EN")]
    [InlineData("en")]
    public void Get_WhenLangIsEn_ReturnsEnAndShowsBgToggle(string lang)
    {
        var sut = CreateSut(new()
        {
            ["ClubName"] = "Club",
            ["Goals_1"] = "Goal 1",
            ["Means_1"] = "Means 1",
            ["MembershipDetails"] = "M",
            ["SupportDetails"] = "S",
        });

        var vm = sut.Get(lang);

        Assert.Equal("EN", vm.Lang);
        Assert.Equal("BG", vm.ChangeLanguageButtonText);
        Assert.Equal("Club", vm.ClubName);
        Assert.Contains("Goal 1", vm.Goals);
    }
}
