using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BitcoinClub.Infrastructure.Auth.Providers
{
    public class EmailPasswordAuthProvider : IAuthProvider
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public EmailPasswordAuthProvider(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public string Name => "email_password";

        public async Task<AuthSignInResult> SignInAsync(AuthSignInRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var result = await _signInManager.PasswordSignInAsync(
                request.Email,
                request.Password,
                request.IsPersistent,
                lockoutOnFailure: true);

            return new AuthSignInResult(
                result.Succeeded,
                result.RequiresTwoFactor,
                result.IsLockedOut,
                result.Succeeded ? null : "Invalid login attempt.");
        }

        public async Task<AuthSignOutResult> SignOutAsync(CancellationToken cancellationToken = default)
        {
            await _signInManager.SignOutAsync();
            return new AuthSignOutResult(true, null);
        }
    }
}
