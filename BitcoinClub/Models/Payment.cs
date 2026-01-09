using System;
using Microsoft.AspNetCore.Identity;

namespace BitcoinClub.Models
{
    public sealed class Payment
    {
        public Guid Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public IdentityUser? User { get; set; }

        public Guid SubscriptionId { get; set; }

        public Subscription? Subscription { get; set; }

        public string Provider { get; set; } = "breez";

        public string ProviderPaymentId { get; set; } = string.Empty;

        public int AmountSats { get; set; }

        public string? PaymentRequest { get; set; }

        public string Status { get; set; } = "initiated";

        public DateTime CreatedAt { get; set; }

        public DateTime? PaidAt { get; set; }
    }
}
