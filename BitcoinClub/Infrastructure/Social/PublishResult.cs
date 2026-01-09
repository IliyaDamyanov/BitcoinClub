namespace BitcoinClub.Infrastructure.Social
{
    public sealed record PublishResult(bool Success, string? ProviderPostId, string? Error);
}
