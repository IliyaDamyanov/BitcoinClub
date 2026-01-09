namespace BitcoinClub.Infrastructure.Payments
{
    public sealed class BreezOptions
    {
        public const string SectionName = "Breez";

        public string ApiKey { get; set; } = string.Empty;

        public string Environment { get; set; } = "test";

        public string? DefaultNodeUrl { get; set; }
    }
}
