namespace BitcoinClub.Infrastructure.Auth.Providers
{
    public sealed record AuthSignInRequest(string Email, string Password, bool IsPersistent = false);
}
