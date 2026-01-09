namespace BitcoinClub.ViewModels
{
    public sealed class LandingPageViewModel
    {
        public string ClubName { get; set; } = "Bitcoin Club";

        public string Description { get; set; } = "A community for learning, building, and using Bitcoin.";

        public string Mission { get; set; } = "Help members build Bitcoin skills through meetups, workshops, and real-world practice.";

        public string ContactEmail { get; set; } = "contact@bitcoinclub.local";

        public string? ContactTelegram { get; set; } = null;
    }
}
