using System;
using System.Linq;
using BitcoinClub.Data;
using BitcoinClub.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BitcoinClub.Tests.Subscriptions
{
    public class SubscriptionMappingTests
    {
        [Fact]
        public void Subscription_IsInModel_AndHasFkToAspNetUsers()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql("Host=localhost;Database=bitcoinclubdb;Username=ignored;Password=ignored")
                .Options;

            using var ctx = new ApplicationDbContext(options);

            var entity = ctx.Model.FindEntityType(typeof(Subscription));
            Assert.NotNull(entity);

            var fk = entity!.GetForeignKeys().SingleOrDefault(f =>
                f.Properties.Any(p => p.Name == nameof(Subscription.UserId)));

            Assert.NotNull(fk);
            Assert.Equal("AspNetUsers", fk!.PrincipalEntityType.GetTableName());
        }
    }
}
