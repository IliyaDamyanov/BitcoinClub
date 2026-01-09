using BitcoinClub.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BitcoinClub.Tests.Database
{
    public class DbContextConfigurationTests
    {
        [Fact]
        public void ApplicationDbContext_CanBeConfiguredWithNpgsqlProvider()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql("Host=localhost;Database=bitcoinclub;Username=bitcoinclub;Password=change-me")
                .Options;

            using var ctx = new ApplicationDbContext(options);
            Assert.Equal("Npgsql.EntityFrameworkCore.PostgreSQL", ctx.Database.ProviderName);
        }
    }
}
