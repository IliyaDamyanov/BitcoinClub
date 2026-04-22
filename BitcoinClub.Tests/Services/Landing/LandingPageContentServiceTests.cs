using System.Globalization;

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
    public void Get_WhenUICultureIsBg_ReturnsBgAndShowsEnToggle()
    {
        var sut = CreateSut(new()
        {
            ["ClubName"] = "����",
            ["Goals_1"] = "��� 1",
            ["Means_1"] = "�������� 1",
            ["MembershipDetails"] = "�",
            ["SupportDetails"] = "�",
        });

        CultureInfo.CurrentUICulture = new CultureInfo("bg");
        var vm = sut.Get();

        Assert.Equal("BG", vm.Lang);
        Assert.Equal("EN", vm.ChangeLanguageButtonText);
        Assert.Equal("����", vm.ClubName);
        Assert.Contains("��� 1", vm.Goals);
    }

    [Fact]
    public void Get_WhenUICultureIsEn_ReturnsEnAndShowsBgToggle()
    {
        var sut = CreateSut(new()
        {
            ["ClubName"] = "Club",
            ["Goals_1"] = "Goal 1",
            ["Means_1"] = "Means 1",
            ["MembershipDetails"] = "M",
            ["SupportDetails"] = "S",
        });

        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var vm = sut.Get();

        Assert.Equal("EN", vm.Lang);
        Assert.Equal("BG", vm.ChangeLanguageButtonText);
        Assert.Equal("Club", vm.ClubName);
        Assert.Contains("Goal 1", vm.Goals);
    }
}
