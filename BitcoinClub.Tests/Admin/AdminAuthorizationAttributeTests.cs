using System;
using System.Linq;
using BitcoinClub.Areas.Admin.Controllers;
using BitcoinClub.Infrastructure.Auth;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace BitcoinClub.Tests.Admin
{
    public class AdminAuthorizationAttributeTests
    {
        [Fact]
        public void AdminControllers_HaveAuthorizeAttributeWithAdminRole()
        {
            var controllerTypes = typeof(HomeController).Assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => t.Namespace == typeof(HomeController).Namespace)
                .Where(t => t.Name.EndsWith("Controller", StringComparison.Ordinal));

            foreach (var t in controllerTypes)
            {
                var authorize = t.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
                    .Cast<AuthorizeAttribute>()
                    .ToArray();

                Assert.True(authorize.Length > 0, $"{t.FullName} must have an AuthorizeAttribute.");
                Assert.Contains(authorize, a => string.Equals(a.Roles, RoleNames.Admin, StringComparison.Ordinal));
            }
        }
    }
}
