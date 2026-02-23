namespace BitcoinClub.ViewModels
{
    public sealed class LandingPageViewModel
    {
        public string Lang { get; set; } = "BG";

        public string ClubName { get; set; } = "������� ����";

        public string HomeLabel { get; set; } = "������";

        public string EventsLabel { get; set; } = "�������";

        public string InfoLabel { get; set; } = "���������� �� �����������";

        public string UsefulLinksLabel { get; set; } = "������� �������";

        public string MembershipAndSupportLabel { get; set; } = "�������� � ��������";

        public string MissionTitle { get; set; } = "Мисия";

        public string GoalsTitle { get; set; } = "����";

        public string[] Goals { get; set; } =
        [
            "�������������� � ����������� � �������� �� �������������� � ������� ����� �������;",
            "��������� �� ������� �� �����������, ������������ � ���������� �� ������������ � ������� ����� �������;",
            "����������� �� ���������� � ����������� �� �������� � ����������� ����� �� �������������� � �������;",
            "��������� ���������� �� ������ ������� � �� ��������� ������� ��������, ������������ � ������;",
            "����������� �� ������ � ��������, ����� � ��������� �� ����������� �������� �� ���������� ����������;",
            "���������� � ���������� �� ���� ������� ��� ������ ��������;",
            "�� ��������� �������������� � ���������� ����� ��������� � ���������;",
            "�������� � ����������� �� ���������� �� ������� � ����� ������������ ���� ������������ �������."
        ];

        public string MeansTitle { get; set; } = "�������� �� ��������� �� ������:";

        public string[] Means { get; set; } =
        [
            "������������ �� �����, �����������, ��������, ��������, ������������� �������, ����������� � ����� ������� �����������;",
            "��������� � ���������������� �� �������� ��������� � ������� (�����, ���������, ��������) � ���������� �� ���������� �� ����������� �� �������� �� �������� � �� ���� �������;",
            "�������� �� ������������� � ������������� ��������� �� ���� ������������ � �������;",
            "���������� �� ����������� � ������� � �������� �� �������������� � �������;",
            "������������� � ���������� �� ������������� ����� � ��������� � ������� ��������� ��� ������ �������� � �������� � �������;",
            "����� �������, ������ ������ � ����������� �� ������ � �������� � ������ � ���������� �� ���������."
        ];

        public string CalendarTitle { get; set; } = "�������� �� ������� ������� � ��������";

        public string CalendarEmbedUrl { get; set; } = "https://calendar.google.com/calendar/embed?src=ef1b943ddbe088897fb86e9ddac3a57f3bbc0f303c93f6608369ff1926fc97cc%40group.calendar.google.com&ctz=Europe%2FSofia";

        public string AssociationTitle { get; set; } = "��������� ������� ����";

        public string AssociationEik { get; set; } = "208038371";

        public string AssociationAddressLine { get; set; } = "��. �����, �.�. 1111, �.�. ���� �����, ��. �����. ������ ������ � 30";

        public string AssociationLocationLabel { get; set; } = "�������";

        public string AssociationLocationUrl { get; set; } = "https://maps.app.goo.gl/t3J85eSAowYXqP7S8";

        public string OfficialWebsiteLabel { get; set; } = "��������� ����";

        public string OfficialWebsiteUrl { get; set; } = "http://bitcoinclub.bg";

        public string SocialMediaTitle { get; set; } = "�������� �����";

        public (string Name, string Url)[] SocialLinks { get; set; } =
        [
            ("Facebook", "https://www.facebook.com/bitcoinclubbg"),
            ("Instagram", "https://www.instagram.com/bitcoinclubbg"),
            ("Twitter", "https://twitter.com/bitcoinclubbg"),
            ("YouTube", "https://www.youtube.com/@bitcoinclubbg")
        ];

        public string MembershipDetails { get; set; } = "��� ������ �� ������� ���� �� ������� ����, ���� �� �� �������� �� ��������� ����� �� ������ �����������.";

        public string SupportDetails { get; set; } = "��� ������� �� �� ����������: bitcoinclub@walletofsatoshi.com";

        public string ContactsTitle { get; set; } = "��������";

        public string ContactEmail { get; set; } = "bitcoinclubbg@gmail.com";

        public string ContactPhone { get; set; } = "0878 413 688";

        public string UsefulLinksTitle { get; set; } = "������� �������";

        public (string Label, string Url)[] UsefulLinks { get; set; } =
        [
            ("YouTube ����� �� ������ �������", "https://www.youtube.com/PlamenAndonov"),
            ("YouTube ����� �� ���� ������", "https://www.youtube.com/@bgcryptonetwork"),
            ("���������� ����� �� Bitcoin Club � ������� ������� �� ������ �������", "https://discord.com/channels/798854534537543701/1163216416674168952"),
            ("�������� ������� Bitcoiners Bulgaria", "https://t.me/+YqOiz2O6xZc4YTM0"),
            ("Website �� ���� ������� � ������� �������� � ����� �����", "https://www.blindspotbg.com/bg"),
            ("������������� ����, �������� �� ���������", "https://studybitcoin.ic.bike"),
            ("������� �����, ��������� �� ��������� ����", "https://bitcoinready.bg/"),
            ("����� � ��������, �������� �������", "https://btcmap.org/"),
        ];

        public string ChangeLanguageButtonText { get; set; } = "EN";

        public string HeroImageRelativePath { get; set; } = "/img/btclogo.png";
    }
}
