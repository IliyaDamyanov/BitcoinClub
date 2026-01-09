using System;
using System.Linq;
using BitcoinClub.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BitcoinClub.Tests.SocialMedia
{
    public class PostEfMappingTests
    {
        [Fact]
        public void Post_Maps_ImagePathsAndPlatforms_AsJsonb()
        {
            using var db = CreateInMemoryDb();
            var entity = db.Model.FindEntityType("BitcoinClub.Models.Post");
            Assert.NotNull(entity);

            var imagePaths = entity!.FindProperty("ImagePaths");
            var platforms = entity.FindProperty("Platforms");

            Assert.Equal("jsonb", imagePaths!.GetColumnType());
            Assert.Equal("jsonb", platforms!.GetColumnType());
        }

        private static ApplicationDbContext CreateInMemoryDb()
        {
            var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
                .Options;

            return new ApplicationDbContext(opts);
        }
    }
}
