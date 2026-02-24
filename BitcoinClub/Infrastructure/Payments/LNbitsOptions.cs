namespace BitcoinClub.Infrastructure.Payments
{
    public sealed class LNbitsOptions
    {
        public const string SectionName = "LNbits";

        public string BaseUrl { get; set; } = "https://legend.lnbits.com";

        public string ApiKey { get; set; } = string.Empty;
    }
}
