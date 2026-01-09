using System;
using BitcoinClub.Models;
using Xunit;

namespace BitcoinClub.Tests.Subscriptions
{
    public class SubscriptionModelTests
    {
        [Fact]
        public void CanCreateSubscription_WithExpectedDefaults()
        {
            var s = new Subscription();

            Assert.Equal(Guid.Empty, s.Id);
            Assert.Equal(string.Empty, s.UserId);
            Assert.Null(s.User);
            Assert.Equal(default, s.ExpirationDate);
            Assert.Null(s.LastPaymentDate);
            Assert.Equal(default, s.CreatedAt);
        }

        [Fact]
        public void CanAssignFields()
        {
            var now = DateTime.UtcNow;
            var exp = now.AddDays(30);

            var s = new Subscription
            {
                Id = Guid.NewGuid(),
                UserId = "u1",
                CreatedAt = now,
                ExpirationDate = exp,
                LastPaymentDate = now
            };

            Assert.Equal("u1", s.UserId);
            Assert.Equal(exp, s.ExpirationDate);
            Assert.Equal(now, s.LastPaymentDate);
            Assert.Equal(now, s.CreatedAt);
        }
    }
}
