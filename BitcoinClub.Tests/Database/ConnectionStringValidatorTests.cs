using System;
using BitcoinClub.Infrastructure.Database;
using Xunit;

namespace BitcoinClub.Tests.Database
{
    public class ConnectionStringValidatorTests
    {
        [Fact]
        public void ValidatePostgresConnectionString_Empty_Throws()
        {
            Assert.Throws<ArgumentException>(() => ConnectionStringValidator.ValidatePostgresConnectionString(""));
        }

        [Fact]
        public void ValidatePostgresConnectionString_MissingHost_Throws()
        {
            var cs = "Database=bitcoinclub;Username=bitcoinclub;Password=change-me";
            Assert.Throws<ArgumentException>(() => ConnectionStringValidator.ValidatePostgresConnectionString(cs));
        }

        [Fact]
        public void ValidatePostgresConnectionString_MissingDatabase_Throws()
        {
            var cs = "Host=localhost;Username=bitcoinclub;Password=change-me";
            Assert.Throws<ArgumentException>(() => ConnectionStringValidator.ValidatePostgresConnectionString(cs));
        }

        [Fact]
        public void ValidatePostgresConnectionString_MissingUsername_Throws()
        {
            var cs = "Host=localhost;Database=bitcoinclub;Password=change-me";
            Assert.Throws<ArgumentException>(() => ConnectionStringValidator.ValidatePostgresConnectionString(cs));
        }

        [Theory]
        [InlineData("Host=localhost;Database=bitcoinclub;Username=bitcoinclub;Password=change-me")]
        [InlineData("Host=localhost;Database=bitcoinclub;User ID=bitcoinclub;Password=change-me")]
        public void ValidatePostgresConnectionString_Valid_DoesNotThrow(string cs)
        {
            ConnectionStringValidator.ValidatePostgresConnectionString(cs);
        }
    }
}
