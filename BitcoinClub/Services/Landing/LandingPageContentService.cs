// LandingPageStrings is in the BitcoinClub namespace
using System.Globalization;
using BitcoinClub.ViewModels;
using Microsoft.Extensions.Localization;

namespace BitcoinClub.Services.Landing;

public sealed class LandingPageContentService : ILandingPageContentService
{
    private readonly IStringLocalizer<LandingPageStrings> _localizer;

    public LandingPageContentService(IStringLocalizer<LandingPageStrings> localizer)
    {
        _localizer = localizer;
    }

    public LandingPageViewModel Get()
    {
        // CultureInfo.CurrentUICulture is set by UseRequestLocalization middleware
        // from the culture cookie before the controller runs.
        var normalized = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName
            .Equals("en", StringComparison.OrdinalIgnoreCase) ? "EN" : "BG";

        var vm = CreateLocalized();
        vm.Lang = normalized;
        vm.ChangeLanguageButtonText = normalized == "EN" ? "BG" : "EN";

        return vm;
    }

    private LandingPageViewModel CreateLocalized()
        => new()
        {
            ClubName = _localizer["ClubName"],
            MissionTitle = _localizer["MissionTitle"],
            HomeLabel = _localizer["HomeLabel"],
            EventsLabel = _localizer["EventsLabel"],
            InfoLabel = _localizer["InfoLabel"],
            UsefulLinksLabel = _localizer["UsefulLinksLabel"],
            MembershipLabel = _localizer["MembershipLabel"],
            MembershipAndSupportLabel = _localizer["MembershipAndSupportLabel"],
            RegisterLabel = _localizer["RegisterLabel"],
            LoginLabel = _localizer["LoginLabel"],
            LogoutLabel = _localizer["LogoutLabel"],
            GoalsTitle = _localizer["GoalsTitle"],
            Goals =
            [
                _localizer["Goals_1"],
                _localizer["Goals_2"],
                _localizer["Goals_3"],
                _localizer["Goals_4"],
                _localizer["Goals_5"],
                _localizer["Goals_6"],
                _localizer["Goals_7"],
                _localizer["Goals_8"],
            ],
            MeansTitle = _localizer["MeansTitle"],
            Means =
            [
                _localizer["Means_1"],
                _localizer["Means_2"],
                _localizer["Means_3"],
                _localizer["Means_4"],
                _localizer["Means_5"],
                _localizer["Means_6"],
            ],
            StatMembersLabel = _localizer["StatMembersLabel"],
            StatEventsLabel = _localizer["StatEventsLabel"],
            StatYearsLabel = _localizer["StatYearsLabel"],
            CalendarTitle = _localizer["CalendarTitle"],
            AssociationTitle = _localizer["AssociationTitle"],
            AssociationAddressLine = _localizer["AssociationAddressLine"],
            AssociationLocationLabel = _localizer["AssociationLocationLabel"],
            OfficialWebsiteLabel = _localizer["OfficialWebsiteLabel"],
            SocialMediaTitle = _localizer["SocialMediaTitle"],
            MembershipDetails = _localizer["MembershipDetails"],
            SupportDetails = _localizer["SupportDetails"],
            ContactsTitle = _localizer["ContactsTitle"],
            UsefulLinksTitle = _localizer["UsefulLinksTitle"],
            UsefulLinks =
            [
                (_localizer["UsefulLink_PlamenAndonov"], "https://www.youtube.com/PlamenAndonov"),
                (_localizer["UsefulLink_BGCrypto"],      "https://www.youtube.com/@bgcryptonetwork"),
                (_localizer["UsefulLink_Discord"],       "https://discord.com/channels/798854534537543701/1163216416674168952"),
                (_localizer["UsefulLink_Telegram"],      "https://t.me/+YqOiz2O6xZc4YTM0"),
                (_localizer["UsefulLink_BlindSpot"],     "https://www.blindspotbg.com/bg"),
                (_localizer["UsefulLink_LearnMeBitcoin"], "https://learnmeabitcoin.com"),
                (_localizer["UsefulLink_BitcoinReady"],  "https://bitcoinready.bg/"),
                (_localizer["UsefulLink_BTCMap"],        "https://btcmap.org/"),
            ],
        };
}
