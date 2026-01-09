using System;

namespace BitcoinClub.ViewModels
{
    public class MembershipDashboardViewModel
    {
        public DateTime ExpirationDate { get; init; }

        public DateTime? LastPaymentDate { get; init; }
    }
}
