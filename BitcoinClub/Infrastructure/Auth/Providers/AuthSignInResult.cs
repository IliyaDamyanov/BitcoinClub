namespace BitcoinClub.Infrastructure.Auth.Providers
{
    public sealed record AuthSignInResult(bool Succeeded, bool RequiresTwoFactor, bool IsLockedOut, string? Error);
}
