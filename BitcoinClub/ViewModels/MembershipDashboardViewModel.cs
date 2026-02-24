using System;
using System.Collections.Generic;

namespace BitcoinClub.ViewModels
{
    public class MembershipDashboardViewModel
    {
        public DateTime ExpirationDate { get; init; }

        public DateTime? LastPaymentDate { get; init; }

        public bool IsActive => ExpirationDate > DateTime.UtcNow;

        public List<PaymentHistoryItem> PaymentHistory { get; init; } = new();
    }

    public class PaymentHistoryItem
    {
        public DateTime CreatedAt { get; init; }

        public int AmountSats { get; init; }

        public string Status { get; init; } = string.Empty;

        public DateTime? PaidAt { get; init; }
    }
}
