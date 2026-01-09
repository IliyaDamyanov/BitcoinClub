using Microsoft.AspNetCore.Authorization;

namespace BitcoinClub.Infrastructure.Auth
{
    public class RequireRoleAttribute : AuthorizeAttribute
    {
        public RequireRoleAttribute(string role)
        {
            Roles = role;
        }
    }
}
