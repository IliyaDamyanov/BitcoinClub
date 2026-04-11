namespace BitcoinClub.ViewModels
{
    public sealed class LandingPageViewModel
    {
        public string Lang { get; set; } = "BG";

        // All text fields below are populated by LandingPageContentService from .resx localizer.
        // Defaults are empty strings — the service always sets them.
        public string ClubName { get; set; } = "";
        public string HomeLabel { get; set; } = "";
        public string EventsLabel { get; set; } = "";
        public string InfoLabel { get; set; } = "";
        public string UsefulLinksLabel { get; set; } = "";
        public string MembershipLabel { get; set; } = "";
        public string MembershipAndSupportLabel { get; set; } = "";
        public string RegisterLabel { get; set; } = "";
        public string LoginLabel { get; set; } = "";
        public string LogoutLabel { get; set; } = "";
        public string MissionTitle { get; set; } = "";
        public string GoalsTitle { get; set; } = "";
        public string[] Goals { get; set; } = [];
        public string MeansTitle { get; set; } = "";
        public string[] Means { get; set; } = [];
        public string CalendarTitle { get; set; } = "";
        public string AssociationTitle { get; set; } = "";
        public string AssociationAddressLine { get; set; } = "";
        public string AssociationLocationLabel { get; set; } = "";
        public string OfficialWebsiteLabel { get; set; } = "";
        public string SocialMediaTitle { get; set; } = "";
        public string MembershipDetails { get; set; } = "";
        public string SupportDetails { get; set; } = "";
        public string ContactsTitle { get; set; } = "";
        public string UsefulLinksTitle { get; set; } = "";
        public string StatMembersLabel { get; set; } = "";
        public string StatEventsLabel { get; set; } = "";
        public string StatYearsLabel { get; set; } = "";

        // Non-localizable data — URLs, IDs, contact info, embedded content.
        public string CalendarEmbedUrl { get; set; } = "https://calendar.google.com/calendar/embed?src=ef1b943ddbe088897fb86e9ddac3a57f3bbc0f303c93f6608369ff1926fc97cc%40group.calendar.google.com&ctz=Europe%2FSofia";
        public string AssociationEik { get; set; } = "208038371";
        public string AssociationLocationUrl { get; set; } = "https://maps.app.goo.gl/t3J85eSAowYXqP7S8";
        public string OfficialWebsiteUrl { get; set; } = "http://bitcoinclub.bg";
        public string ContactEmail { get; set; } = "bitcoinclubbg@gmail.com";
        public string ContactPhone { get; set; } = "0878 413 688";
        public string HeroImageRelativePath { get; set; } = "/img/btclogo.png";
        public string ChangeLanguageButtonText { get; set; } = "EN";

        public (string Name, string Url)[] SocialLinks { get; set; } =
        [
            ("Facebook", "https://www.facebook.com/bitcoinclubbg"),
            ("Instagram", "https://www.instagram.com/bitcoinclubbg"),
            ("Twitter", "https://twitter.com/bitcoinclubbg"),
            ("YouTube", "https://www.youtube.com/@bitcoinclubbg")
        ];

        // Set by LandingPageContentService — labels come from .resx, URLs are hardcoded.
        public (string Label, string Url)[] UsefulLinks { get; set; } = [];
    }
}
