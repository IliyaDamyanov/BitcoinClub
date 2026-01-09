using System.IO;
using System.Linq;
using Xunit;

namespace BitcoinClub.Tests.Database
{
    public class MigrationExistenceTests
    {
        [Fact]
        public void InitialCreateMigration_ExistsOnDisk()
        {
            var migrationsDir = Path.Combine("..", "..", "..", "..", "BitcoinClub", "Data", "Migrations");
            Assert.True(Directory.Exists(migrationsDir));

            var files = Directory.GetFiles(migrationsDir, "*_InitialCreate.cs").Select(Path.GetFileName).ToArray();
            Assert.Single(files);
        }
    }
}
