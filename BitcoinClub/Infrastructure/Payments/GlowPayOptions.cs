namespace BitcoinClub.Infrastructure.Payments
{
    public sealed class GlowPayOptions
    {
        public const string SectionName = "GlowPay";

        public string BaseUrl { get; set; } = "https://glow-pay.co";

        public string ApiKey { get; set; } = string.Empty;

        public string WebhookSecret { get; set; } = string.Empty;
    }
}
