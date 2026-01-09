using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BitcoinClub.Data;
using BitcoinClub.Infrastructure.Social;
using BitcoinClub.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BitcoinClub.Tests.SocialMedia
{
    public class SocialMediaPublishManagerUnitTests
    {
        [Fact]
        public async Task PublishAsync_NullPost_Throws()
        {
            var db = CreateInMemoryDb();
            var sut = new SocialMediaPublishManager(db, Array.Empty<ISocialMediaPublisher>());

            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.PublishAsync(null!));
        }

        [Fact]
        public async Task PublishAsync_NoPlatforms_DoesNothing()
        {
            var db = CreateInMemoryDb();
            var sut = new SocialMediaPublishManager(db, Array.Empty<ISocialMediaPublisher>());

            var post = new Post { Id = Guid.NewGuid(), AdminUserId = "a", TextContent = "t", CreatedAt = DateTime.UtcNow };

            await sut.PublishAsync(post);

            Assert.Empty(db.PostPublishResults);
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
