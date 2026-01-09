using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BitcoinClub.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace BitcoinClub.Tests.Auth
{
    public class DefaultRoleAssignmentTests
    {
        [Fact]
        public async Task GenerateClaimsAsync_WhenUserHasNoRoles_AddsMemberRole()
        {
            var services = BuildServices();

            var user = new IdentityUser { Id = "u1", Email = "a@b.com", UserName = "a@b.com" };
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var factory = services.GetRequiredService<IUserClaimsPrincipalFactory<IdentityUser>>();

            var create = await userManager.CreateAsync(user);
            Assert.True(create.Succeeded);

            var principal = await factory.CreateAsync(user);

            Assert.Contains(principal.Claims, c => c.Type == ClaimTypes.Role && c.Value == RoleNames.Member);
            Assert.True(await userManager.IsInRoleAsync(user, RoleNames.Member));
        }

        [Fact]
        public async Task GenerateClaimsAsync_WhenUserAlreadyHasRole_DoesNotAddMemberRole()
        {
            var services = BuildServices();

            var user = new IdentityUser { Id = "u1", Email = "a@b.com", UserName = "a@b.com" };
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var roleStore = services.GetRequiredService<IRoleStore<IdentityRole>>();
            var factory = services.GetRequiredService<IUserClaimsPrincipalFactory<IdentityUser>>();

            await roleStore.CreateAsync(new IdentityRole(RoleNames.Admin), default);

            var create = await userManager.CreateAsync(user);
            Assert.True(create.Succeeded);

            var addAdmin = await userManager.AddToRoleAsync(user, RoleNames.Admin);
            Assert.True(addAdmin.Succeeded);

            var principal = await factory.CreateAsync(user);

            Assert.DoesNotContain(principal.Claims, c => c.Type == ClaimTypes.Role && c.Value == RoleNames.Member);
            Assert.True(await userManager.IsInRoleAsync(user, RoleNames.Admin));
        }

        private static ServiceProvider BuildServices()
        {
            var services = new ServiceCollection();

            services.AddLogging();
            services.AddSingleton<IOptions<IdentityOptions>>(Options.Create(new IdentityOptions()));
            services.AddSingleton<IdentityErrorDescriber>();

            services.AddSingleton<IUserStore<IdentityUser>, InMemoryUserStore>();
            services.AddSingleton<IRoleStore<IdentityRole>, InMemoryRoleStore>();
            services.AddSingleton<ILookupNormalizer, UpperInvariantLookupNormalizer>();
            services.AddSingleton<IPasswordHasher<IdentityUser>, PasswordHasher<IdentityUser>>();
            services.AddSingleton<IUserValidator<IdentityUser>, UserValidator<IdentityUser>>();
            services.AddSingleton<IPasswordValidator<IdentityUser>, PasswordValidator<IdentityUser>>();

            services.AddSingleton<UserManager<IdentityUser>>();
            services.AddSingleton<IUserClaimsPrincipalFactory<IdentityUser>, DefaultRoleUserClaimsPrincipalFactory>();

            return services.BuildServiceProvider();
        }

        private sealed class UpperInvariantLookupNormalizer : ILookupNormalizer
        {
            public string? NormalizeEmail(string? email) => email?.ToUpperInvariant();
            public string? NormalizeName(string? name) => name?.ToUpperInvariant();
        }

        private sealed class InMemoryRoleStore : IRoleStore<IdentityRole>
        {
            private readonly Dictionary<string, IdentityRole> _roles = new(StringComparer.OrdinalIgnoreCase);

            public Task<IdentityResult> CreateAsync(IdentityRole role, System.Threading.CancellationToken cancellationToken)
            {
                _roles[role.Name ?? role.Id] = role;
                return Task.FromResult(IdentityResult.Success);
            }

            public Task<IdentityResult> UpdateAsync(IdentityRole role, System.Threading.CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
            public Task<IdentityResult> DeleteAsync(IdentityRole role, System.Threading.CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
            public Task<string?> GetRoleIdAsync(IdentityRole role, System.Threading.CancellationToken cancellationToken) => Task.FromResult(role.Id);
            public Task<string?> GetRoleNameAsync(IdentityRole role, System.Threading.CancellationToken cancellationToken) => Task.FromResult(role.Name);
            public Task SetRoleNameAsync(IdentityRole role, string? roleName, System.Threading.CancellationToken cancellationToken) { role.Name = roleName; return Task.CompletedTask; }
            public Task<string?> GetNormalizedRoleNameAsync(IdentityRole role, System.Threading.CancellationToken cancellationToken) => Task.FromResult(role.NormalizedName);
            public Task SetNormalizedRoleNameAsync(IdentityRole role, string? normalizedName, System.Threading.CancellationToken cancellationToken) { role.NormalizedName = normalizedName; return Task.CompletedTask; }

            public Task<IdentityRole?> FindByIdAsync(string roleId, System.Threading.CancellationToken cancellationToken)
            {
                foreach (var r in _roles.Values)
                {
                    if (string.Equals(r.Id, roleId, StringComparison.OrdinalIgnoreCase))
                    {
                        return Task.FromResult<IdentityRole?>(r);
                    }
                }
                return Task.FromResult<IdentityRole?>(null);
            }

            public Task<IdentityRole?> FindByNameAsync(string normalizedRoleName, System.Threading.CancellationToken cancellationToken)
            {
                _roles.TryGetValue(normalizedRoleName, out var role);
                return Task.FromResult<IdentityRole?>(role);
            }

            public void Dispose() { }
        }

        private sealed class InMemoryUserStore : IUserStore<IdentityUser>, IUserEmailStore<IdentityUser>, IUserRoleStore<IdentityUser>, IUserClaimStore<IdentityUser>
        {
            private readonly Dictionary<string, IdentityUser> _users = new(StringComparer.OrdinalIgnoreCase);
            private readonly Dictionary<string, HashSet<string>> _userRoles = new(StringComparer.OrdinalIgnoreCase);

            public Task<IdentityResult> CreateAsync(IdentityUser user, System.Threading.CancellationToken cancellationToken)
            {
                _users[user.Id] = user;
                return Task.FromResult(IdentityResult.Success);
            }

            public Task<IdentityResult> UpdateAsync(IdentityUser user, System.Threading.CancellationToken cancellationToken)
            {
                _users[user.Id] = user;
                return Task.FromResult(IdentityResult.Success);
            }

            public Task<IdentityResult> DeleteAsync(IdentityUser user, System.Threading.CancellationToken cancellationToken)
            {
                _users.Remove(user.Id);
                _userRoles.Remove(user.Id);
                return Task.FromResult(IdentityResult.Success);
            }

            public Task<IdentityUser?> FindByIdAsync(string userId, System.Threading.CancellationToken cancellationToken)
            {
                _users.TryGetValue(userId, out var user);
                return Task.FromResult<IdentityUser?>(user);
            }

            public Task<IdentityUser?> FindByNameAsync(string normalizedUserName, System.Threading.CancellationToken cancellationToken)
            {
                foreach (var u in _users.Values)
                {
                    if (string.Equals(u.NormalizedUserName, normalizedUserName, StringComparison.OrdinalIgnoreCase))
                    {
                        return Task.FromResult<IdentityUser?>(u);
                    }
                }
                return Task.FromResult<IdentityUser?>(null);
            }

            public Task<string?> GetUserIdAsync(IdentityUser user, System.Threading.CancellationToken cancellationToken) => Task.FromResult(user.Id);
            public Task<string?> GetUserNameAsync(IdentityUser user, System.Threading.CancellationToken cancellationToken) => Task.FromResult(user.UserName);
            public Task SetUserNameAsync(IdentityUser user, string? userName, System.Threading.CancellationToken cancellationToken) { user.UserName = userName; return Task.CompletedTask; }
            public Task<string?> GetNormalizedUserNameAsync(IdentityUser user, System.Threading.CancellationToken cancellationToken) => Task.FromResult(user.NormalizedUserName);
            public Task SetNormalizedUserNameAsync(IdentityUser user, string? normalizedName, System.Threading.CancellationToken cancellationToken) { user.NormalizedUserName = normalizedName; return Task.CompletedTask; }

            public Task SetEmailAsync(IdentityUser user, string? email, System.Threading.CancellationToken cancellationToken) { user.Email = email; return Task.CompletedTask; }
            public Task<string?> GetEmailAsync(IdentityUser user, System.Threading.CancellationToken cancellationToken) => Task.FromResult(user.Email);
            public Task<bool> GetEmailConfirmedAsync(IdentityUser user, System.Threading.CancellationToken cancellationToken) => Task.FromResult(user.EmailConfirmed);
            public Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed, System.Threading.CancellationToken cancellationToken) { user.EmailConfirmed = confirmed; return Task.CompletedTask; }
            public Task<IdentityUser?> FindByEmailAsync(string normalizedEmail, System.Threading.CancellationToken cancellationToken)
            {
                foreach (var u in _users.Values)
                {
                    if (string.Equals(u.NormalizedEmail, normalizedEmail, StringComparison.OrdinalIgnoreCase))
                    {
                        return Task.FromResult<IdentityUser?>(u);
                    }
                }
                return Task.FromResult<IdentityUser?>(null);
            }
            public Task<string?> GetNormalizedEmailAsync(IdentityUser user, System.Threading.CancellationToken cancellationToken) => Task.FromResult(user.NormalizedEmail);
            public Task SetNormalizedEmailAsync(IdentityUser user, string? normalizedEmail, System.Threading.CancellationToken cancellationToken) { user.NormalizedEmail = normalizedEmail; return Task.CompletedTask; }

            public Task AddToRoleAsync(IdentityUser user, string roleName, System.Threading.CancellationToken cancellationToken)
            {
                if (!_userRoles.TryGetValue(user.Id, out var roles))
                {
                    roles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    _userRoles[user.Id] = roles;
                }

                roles.Add(roleName);
                return Task.CompletedTask;
            }

            public Task RemoveFromRoleAsync(IdentityUser user, string roleName, System.Threading.CancellationToken cancellationToken)
            {
                if (_userRoles.TryGetValue(user.Id, out var roles))
                {
                    roles.Remove(roleName);
                }
                return Task.CompletedTask;
            }

            public Task<IList<string>> GetRolesAsync(IdentityUser user, System.Threading.CancellationToken cancellationToken)
            {
                if (_userRoles.TryGetValue(user.Id, out var roles))
                {
                    return Task.FromResult<IList<string>>(new List<string>(roles));
                }

                return Task.FromResult<IList<string>>(new List<string>());
            }

            public Task<bool> IsInRoleAsync(IdentityUser user, string roleName, System.Threading.CancellationToken cancellationToken)
            {
                return Task.FromResult(_userRoles.TryGetValue(user.Id, out var roles) && roles.Contains(roleName));
            }

            public Task<IList<IdentityUser>> GetUsersInRoleAsync(string roleName, System.Threading.CancellationToken cancellationToken) => Task.FromResult<IList<IdentityUser>>(new List<IdentityUser>());

            public Task<IList<Claim>> GetClaimsAsync(IdentityUser user, System.Threading.CancellationToken cancellationToken) => Task.FromResult<IList<Claim>>(new List<Claim>());
            public Task AddClaimsAsync(IdentityUser user, IEnumerable<Claim> claims, System.Threading.CancellationToken cancellationToken) => Task.CompletedTask;
            public Task ReplaceClaimAsync(IdentityUser user, Claim claim, Claim newClaim, System.Threading.CancellationToken cancellationToken) => Task.CompletedTask;
            public Task RemoveClaimsAsync(IdentityUser user, IEnumerable<Claim> claims, System.Threading.CancellationToken cancellationToken) => Task.CompletedTask;
            public Task<IList<IdentityUser>> GetUsersForClaimAsync(Claim claim, System.Threading.CancellationToken cancellationToken) => Task.FromResult<IList<IdentityUser>>(new List<IdentityUser>());

            public void Dispose() { }
        }
    }
}
