using System;
using System.Linq;
using System.Threading.Tasks;
using BitcoinClub.Data;
using BitcoinClub.Infrastructure.Social;
using BitcoinClub.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace BitcoinClub.Tests.SocialMedia
{
    public class SocialMediaPublishManagerDbIntegrationTests
    {
        [Fact]
        public async Task PublishAsync_WritesResultsToDatabase_InPostgres()
        {
            var cs = Environment.GetEnvironmentVariable("BITCOINCLUB_TEST_PG_CONNECTION");
            if (string.IsNullOrWhiteSpace(cs))
            {
                return;
            }

            var services = new ServiceCollection();
            services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(cs));
            services.AddSingleton<ISocialMediaPublisher>(new FakePublisher("nostr", new PublishResult(true, "p1", null)));
            services.AddScoped<ISocialMediaPublishManager, SocialMediaPublishManager>();

            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await db.Database.MigrateAsync();

                var adminUserId = Guid.NewGuid().ToString("N");
                db.Users.Add(new Microsoft.AspNetCore.Identity.IdentityUser { Id = adminUserId, UserName = $"a{adminUserId}@test.local", Email = $"a{adminUserId}@test.local" });
                await db.SaveChangesAsync();

                var post = new Post
                {
                    Id = Guid.NewGuid(),
                    AdminUserId = adminUserId,
                    TextContent = "hello",
                    CreatedAt = DateTime.UtcNow,
                    Platforms = { "nostr" }
                };

                db.Posts.Add(post);
                await db.SaveChangesAsync();

                var mgr = scope.ServiceProvider.GetRequiredService<ISocialMediaPublishManager>();
                await mgr.PublishAsync(post);

                var row = await db.PostPublishResults.SingleAsync(r => r.PostId == post.Id && r.Platform == "nostr");
                Assert.True(row.Success);
                Assert.Equal("p1", row.ProviderPostId);
            }
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

            public Task<PublishResult> PublishAsync(Post post, System.Threading.CancellationToken cancellationToken = default)
                => Task.FromResult(_result);
        }
    }
}
