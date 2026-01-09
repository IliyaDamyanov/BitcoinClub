using System;
using System.Threading;
using System.Threading.Tasks;
using BitcoinClub.Infrastructure.Social;
using BitcoinClub.Models;
using Moq;
using Xunit;

namespace BitcoinClub.Tests.SocialMedia
{
    public class MockPublisherTests
    {
        [Fact]
        public async Task MockPublisher_CanBeUsedForPublishingFlow()
        {
            var post = new Post { Id = Guid.NewGuid(), AdminUserId = "admin", TextContent = "hello", CreatedAt = DateTime.UtcNow };

            var mock = new Mock<ISocialMediaPublisher>();
            mock.SetupGet(m => m.Platform).Returns("nostr");
            mock.Setup(m => m.PublishAsync(post, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PublishResult(true, "p1", null));

            var res = await mock.Object.PublishAsync(post);

            Assert.True(res.Success);
            Assert.Equal("p1", res.ProviderPostId);
            mock.Verify(m => m.PublishAsync(post, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
