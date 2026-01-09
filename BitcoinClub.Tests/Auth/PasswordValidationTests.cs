using Microsoft.AspNetCore.Identity;
using Xunit;

namespace BitcoinClub.Tests.Auth
{
    public class PasswordValidationTests
    {
        [Fact]
        public void DefaultPasswordOptions_RequireNonAlphanumeric_IsTrue()
        {
            var opts = new IdentityOptions();
            Assert.True(opts.Password.RequireNonAlphanumeric);
        }

        [Fact]
        public void DefaultPasswordOptions_RequiredLength_IsAtLeast6()
        {
            var opts = new IdentityOptions();
            Assert.True(opts.Password.RequiredLength >= 6);
        }
    }
}
