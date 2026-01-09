using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BitcoinClub.Infrastructure.Auth
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
        {
            try
            {
                using var scope = services.CreateScope();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                await EnsureRoleAsync(roleManager, RoleNames.Member, cancellationToken);
                await EnsureRoleAsync(roleManager, RoleNames.Admin, cancellationToken);
            }
            catch
            {
                return;
            }
        }

        private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string roleName, CancellationToken cancellationToken)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                return;
            }

            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create role '{roleName}': {string.Join(", ", result.Errors)}");
            }
        }
    }
}
