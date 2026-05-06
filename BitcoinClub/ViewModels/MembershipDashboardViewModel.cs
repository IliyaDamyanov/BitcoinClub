using System;
using System.Collections.Generic;

namespace BitcoinClub.ViewModels
{
    public class MembershipDashboardViewModel
    {
        public DateTime ExpirationDate { get; init; }

        public DateTime? LastPaymentDate { get; init; }

        public bool IsActive => ExpirationDate > DateTime.UtcNow;

        public int RemainingDays => Math.Max(0, (int)Math.Ceiling((ExpirationDate - DateTime.UtcNow).TotalDays));

        public List<PaymentHistoryItem> PaymentHistory { get; init; } = new();
    }

    public class PaymentHistoryItem
    {
        public DateTime CreatedAt { get; init; }

        public int AmountSats { get; init; }

        public string Provider { get; init; } = string.Empty;

        public string Status { get; init; } = string.Empty;

        public DateTime? PaidAt { get; init; }
    }
}
