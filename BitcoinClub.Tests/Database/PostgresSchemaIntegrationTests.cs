using Npgsql;
using Xunit;

namespace BitcoinClub.Tests.Database
{
    public class PostgresSchemaIntegrationTests
    {
        [Fact]
        public async Task Schema_HasAspNetUsersTable_WhenDatabaseIsReachable()
        {
            var cs = Environment.GetEnvironmentVariable("BITCOINCLUB_TEST_PG_CONNECTION");
            if (string.IsNullOrWhiteSpace(cs))
            {
                return;
            }

            await using var conn = new NpgsqlConnection(cs);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "select count(*) from information_schema.tables where table_schema = 'public' and table_name = 'aspnetusers';",
                conn);

            var result = (long)(await cmd.ExecuteScalarAsync() ?? 0L);
            Assert.True(result >= 1);
        }
    }
}
