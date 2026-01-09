using System.Threading;
using System.Threading.Tasks;

namespace BitcoinClub.Infrastructure.Auth.Providers
{
    public interface IAuthProvider
    {
        string Name { get; }

        Task<AuthSignInResult> SignInAsync(AuthSignInRequest request, CancellationToken cancellationToken = default);

        Task<AuthSignOutResult> SignOutAsync(CancellationToken cancellationToken = default);
    }
}
