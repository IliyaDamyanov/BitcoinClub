using System;
using System.Threading.Tasks;
using BitcoinClub.Infrastructure.Auth;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BitcoinClub.Tests.Auth
{
    public class RoleSeederTests
    {
        [Fact]
        public async Task SeedAsync_DatabaseUnavailable_DoesNotThrow()
        {
            var services = new ServiceCollection().BuildServiceProvider();
            await RoleSeeder.SeedAsync(services);
        }
    }
}
