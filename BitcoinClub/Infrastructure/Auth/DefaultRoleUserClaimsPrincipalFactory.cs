using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BitcoinClub.Infrastructure.Auth
{
    public class DefaultRoleUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<IdentityUser>
    {
        public DefaultRoleUserClaimsPrincipalFactory(
            UserManager<IdentityUser> userManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(IdentityUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            var roles = await UserManager.GetRolesAsync(user);
            if (roles.Count == 0)
            {
                var addResult = await UserManager.AddToRoleAsync(user, RoleNames.Member);
                if (addResult.Succeeded)
                {
                    identity.AddClaim(new Claim(identity.RoleClaimType, RoleNames.Member));
                }
            }

            return identity;
        }
    }
}
