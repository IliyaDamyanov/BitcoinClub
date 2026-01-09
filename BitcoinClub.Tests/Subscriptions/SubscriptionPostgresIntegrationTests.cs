using System;
using System.Threading.Tasks;
using BitcoinClub.Data;
using BitcoinClub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BitcoinClub.Tests.Subscriptions
{
    public class SubscriptionPostgresIntegrationTests
    {
        [Fact]
        public async Task CanCreateAndRetrieveSubscription_FromPostgres_WhenConfigured()
        {
            var cs = Environment.GetEnvironmentVariable("BITCOINCLUB_TEST_PG_CONNECTION");
            if (string.IsNullOrWhiteSpace(cs))
            {
                return;
            }

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(cs)
                .Options;

            var userId = Guid.NewGuid().ToString("N");
            var subscriptionId = Guid.NewGuid();

            await using (var ctx = new ApplicationDbContext(options))
            {
                await ctx.Database.MigrateAsync();

                ctx.Users.Add(new IdentityUser { Id = userId, UserName = $"u{userId}@test.local", Email = $"u{userId}@test.local" });

                ctx.Subscriptions.Add(new Subscription
                {
                    Id = subscriptionId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    ExpirationDate = DateTime.UtcNow.AddDays(30),
                    LastPaymentDate = null
                });

                await ctx.SaveChangesAsync();
            }

            await using (var ctx = new ApplicationDbContext(options))
            {
                var loaded = await ctx.Subscriptions.SingleAsync(s => s.Id == subscriptionId);
                Assert.Equal(userId, loaded.UserId);
            }
        }
    }
}
