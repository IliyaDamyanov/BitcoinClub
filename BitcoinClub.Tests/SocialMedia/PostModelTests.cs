using System;
using BitcoinClub.Models;
using Xunit;

namespace BitcoinClub.Tests.SocialMedia
{
    public class PostModelTests
    {
        [Fact]
        public void Post_Defaults_AreInitialized()
        {
            var p = new Post();

            Assert.NotNull(p.ImagePaths);
            Assert.NotNull(p.Platforms);
            Assert.Equal(string.Empty, p.TextContent);
        }

        [Fact]
        public void Post_CanStoreValues()
        {
            var id = Guid.NewGuid();
            var p = new Post
            {
                Id = id,
                AdminUserId = "admin1",
                TextContent = "hello",
                CreatedAt = DateTime.UtcNow,
            };

            p.ImagePaths.Add("/img/1.png");
            p.Platforms.Add("nostr");

            Assert.Equal(id, p.Id);
            Assert.Equal("admin1", p.AdminUserId);
            Assert.Single(p.ImagePaths);
            Assert.Single(p.Platforms);
        }
    }
}
