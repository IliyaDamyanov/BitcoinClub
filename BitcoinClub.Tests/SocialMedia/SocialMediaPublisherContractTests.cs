using System;
using System.Linq;
using System.Threading.Tasks;
using BitcoinClub.Infrastructure.Social;
using BitcoinClub.Models;
using Xunit;

namespace BitcoinClub.Tests.SocialMedia
{
    public class SocialMediaPublisherContractTests
    {
        [Fact]
        public async Task Publishers_HaveUniquePlatformKeys_AndReturnResult()
        {
            ISocialMediaPublisher[] publishers =
            {
                new FacebookPublisher(),
                new InstagramPublisher(),
                new ThreadsPublisher(),
                new TwitterPublisher(),
                new NostrPublisher()
            };

            Assert.Equal(publishers.Length, publishers.Select(p => p.Platform).Distinct(StringComparer.Ordinal).Count());
            Assert.All(publishers, p => Assert.False(string.IsNullOrWhiteSpace(p.Platform)));

            var post = new Post { Id = Guid.NewGuid(), AdminUserId = "admin", TextContent = "hello", CreatedAt = DateTime.UtcNow };

            foreach (var p in publishers)
            {
                var res = await p.PublishAsync(post);
                Assert.NotNull(res);
            }
        }
    }
}
