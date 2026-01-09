using System;
using BitcoinClub.ViewModels;
using Xunit;

namespace BitcoinClub.Tests.Membership
{
    public class MembershipDashboardViewModelTests
    {
        [Fact]
        public void CanCreateViewModel_WithValues()
        {
            var now = DateTime.UtcNow;
            var exp = now.AddDays(30);

            var vm = new MembershipDashboardViewModel
            {
                ExpirationDate = exp,
                LastPaymentDate = now
            };

            Assert.Equal(exp, vm.ExpirationDate);
            Assert.Equal(now, vm.LastPaymentDate);
        }
    }
}
