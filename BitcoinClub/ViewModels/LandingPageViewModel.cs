namespace BitcoinClub.ViewModels
{
    public sealed class LandingPageViewModel
    {
        public string Lang { get; set; } = "BG";

        public string ClubName { get; set; } = "Биткойн Клуб";

        public string HomeLabel { get; set; } = "Начало";

        public string EventsLabel { get; set; } = "Събития";

        public string InfoLabel { get; set; } = "Информация за сдружението";

        public string UsefulLinksLabel { get; set; } = "Полезни линкове";

        public string MembershipAndSupportLabel { get; set; } = "Членство и подкрепа";

        public string GoalsTitle { get; set; } = "Цели";

        public string[] Goals { get; set; } =
        [
            "Популяризиране и образование в областта на криптовалутите с основен фокус Биткойн;",
            "Създаване на общност от потребители, разработчици и ентусиасти на криптовалути с основен фокус Биткойн;",
            "Осигуряване на информация и консултации за правната и регулаторна рамка на криптовалутите и Биткойн;",
            "Обединява интересите на своите членове и да подпомага тяхното развитие, квалификация и знания;",
            "Стимулиране на диалог и дискусии, както и създаване на авторитетен източник на качествена информация;",
            "Обединение и привличане на нови членове със сходни интереси;",
            "Да насърчава сътрудничество и контактите между членовете и общността;",
            "Подкрепа и стимулиране на приемането на Биткойн и други криптовалути като технологично решение."
        ];

        public string MeansTitle { get; set; } = "Средства за постигане на целите:";

        public string[] Means { get; set; } =
        [
            "Организиране на срещи, конференции, семинари, уебинари, образователни курсове, презентации и други подобни мероприятия;",
            "Създаване и разпространяване на рекламни материали в медиите (преса, телевизия, интернет) с информация за дейностите на Сдружението за набиране на средства и на нови членове;",
            "Издаване на информационни и образователни материали на тема криптовалути и Биткойн;",
            "Провеждане на изследвания и анализи в областта на криптовалутите и Биткойн;",
            "Осъществяване и поддържане на информационен обмен и отношения с подобни сдружения със сходни интереси в страната и чужбина;",
            "Друга дейност, когато същата е незабранена от Закона и свързана с целите и средствата на Сдружение."
        ];

        public string CalendarTitle { get; set; } = "Календар на Биткойн събития в България";

        public string CalendarEmbedUrl { get; set; } = "https://calendar.google.com/calendar/embed?src=ef1b943ddbe088897fb86e9ddac3a57f3bbc0f303c93f6608369ff1926fc97cc%40group.calendar.google.com&ctz=Europe%2FSofia";

        public string AssociationTitle { get; set; } = "Сдружение Биткойн Клуб";

        public string AssociationEik { get; set; } = "208038371";

        public string AssociationAddressLine { get; set; } = "гр. София, п.к. 1111, ж.к. „Гео Милев“, ул. „Проф. Георги Павлов“ № 30";

        public string AssociationLocationLabel { get; set; } = "локация";

        public string AssociationLocationUrl { get; set; } = "https://maps.app.goo.gl/t3J85eSAowYXqP7S8";

        public string OfficialWebsiteLabel { get; set; } = "Официален сайт";

        public string OfficialWebsiteUrl { get; set; } = "http://bitcoinclub.bg";

        public string SocialMediaTitle { get; set; } = "Социални мрежи";

        public (string Name, string Url)[] SocialLinks { get; set; } =
        [
            ("Facebook", "https://www.facebook.com/bitcoinclubbg"),
            ("Instagram", "https://www.instagram.com/bitcoinclubbg"),
            ("Twitter", "https://twitter.com/bitcoinclubbg"),
            ("YouTube", "https://www.youtube.com/@bitcoinclubbg")
        ];

        public string MembershipDetails { get; set; } = "Ако искате да станете член на Биткойн клуб, може да се свържете на посочения имейл за повече подробности.";

        public string SupportDetails { get; set; } = "Ако желаете да ни подкрепите: bitcoinclub@walletofsatoshi.com";

        public string ContactsTitle { get; set; } = "Контакти";

        public string ContactEmail { get; set; } = "bitcoinclubbg@gmail.com";

        public string ContactPhone { get; set; } = "0878 413 688";

        public string UsefulLinksTitle { get; set; } = "Полезни линкове";

        public (string Label, string Url)[] UsefulLinks { get; set; } =
        [
            ("YouTube канал на Пламен Андонов", "https://www.youtube.com/PlamenAndonov"),
            ("YouTube канал на Иван Иванов", "https://www.youtube.com/@bgcryptonetwork"),
            ("Публичният канал на Bitcoin Club в дискорд сървъра на Пламен Андонов", "https://discord.com/channels/798854534537543701/1163216416674168952"),
            ("Телеграм групата Bitcoiners Bulgaria", "https://t.me/+YqOiz2O6xZc4YTM0"),
            ("Website на Жоро Николов с биткойн обучения и други видеа", "https://www.blindspotbg.com/bg"),
            ("Образователен сайт, подходящ за начинаещи", "https://studybitcoin.ic.bike"),
            ("Биткойн книги, преведени на български език", "https://bitcoinready.bg/"),
            ("Карта с търговци, приемащи биткойн", "https://btcmap.org/"),
        ];

        public string ChangeLanguageButtonText { get; set; } = "EN";

        public string HeroImageRelativePath { get; set; } = "/img/btclogo.png";
    }
}
