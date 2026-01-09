using System;
using System.Threading;
using System.Threading.Tasks;
using BitcoinClub.Data;
using BitcoinClub.Infrastructure.Social;
using BitcoinClub.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BitcoinClub.Tests.SocialMedia
{
    public class SocialMediaPublishManagerMultiPlatformTests
    {
        [Fact]
        public async Task PublishAsync_MultiplePlatforms_CreatesResultPerPlatform()
        {
            var db = CreateInMemoryDb();

            var publishers = new ISocialMediaPublisher[]
            {
                new FakePublisher("nostr", new PublishResult(true, "n1", null)),
                new FakePublisher("twitter", new PublishResult(false, null, "fail"))
            };

            var sut = new SocialMediaPublishManager(db, publishers);

            var post = new Post
            {
                Id = Guid.NewGuid(),
                AdminUserId = "a",
                TextContent = "t",
                CreatedAt = DateTime.UtcNow,
                Platforms = { "nostr", "twitter" }
            };

            db.Posts.Add(post);
            await db.SaveChangesAsync();

            await sut.PublishAsync(post);

            var results = await db.PostPublishResults.ToListAsync();
            Assert.Equal(2, results.Count);

            Assert.Contains(results, r => r.Platform == "nostr" && r.Success && r.ProviderPostId == "n1");
            Assert.Contains(results, r => r.Platform == "twitter" && !r.Success && r.Error == "fail");
        }

        private static ApplicationDbContext CreateInMemoryDb()
        {
            var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
                .Options;

            return new ApplicationDbContext(opts);
        }

        private sealed class FakePublisher : ISocialMediaPublisher
        {
            private readonly PublishResult _result;

            public FakePublisher(string platform, PublishResult result)
            {
                Platform = platform;
                _result = result;
            }

            public string Platform { get; }

            public Task<PublishResult> PublishAsync(Post post, CancellationToken cancellationToken = default)
                => Task.FromResult(_result);
        }
    }
}
