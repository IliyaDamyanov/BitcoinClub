using System;
using System.Linq;
using System.Threading.Tasks;
using BitcoinClub.Data;
using BitcoinClub.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BitcoinClub.Tests.SocialMedia
{
    public class PostCreateReadIntegrationTests
    {
        [Fact]
        public async Task CanCreateAndReadPost_InPostgres()
        {
            var cs = Environment.GetEnvironmentVariable("BITCOINCLUB_TEST_PG_CONNECTION");
            if (string.IsNullOrWhiteSpace(cs))
            {
                return;
            }

            var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(cs)
                .Options;

            await using var db = new ApplicationDbContext(opts);
            await db.Database.MigrateAsync();

            var adminUserId = Guid.NewGuid().ToString("N");
            db.Users.Add(new Microsoft.AspNetCore.Identity.IdentityUser { Id = adminUserId, UserName = $"a{adminUserId}@test.local", Email = $"a{adminUserId}@test.local" });
            await db.SaveChangesAsync();

            var post = new Post
            {
                Id = Guid.NewGuid(),
                AdminUserId = adminUserId,
                TextContent = "hello world",
                CreatedAt = DateTime.UtcNow,
                ImagePaths = { "/img/one.png" },
                Platforms = { "nostr", "twitter" }
            };

            db.Posts.Add(post);
            await db.SaveChangesAsync();

            var loaded = await db.Posts.SingleAsync(p => p.Id == post.Id);

            Assert.Equal(post.AdminUserId, loaded.AdminUserId);
            Assert.Equal("hello world", loaded.TextContent);
            Assert.Contains("/img/one.png", loaded.ImagePaths);
            Assert.Contains("nostr", loaded.Platforms);
        }
    }
}
