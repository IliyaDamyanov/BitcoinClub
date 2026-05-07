using System;

namespace BitcoinClub.ViewModels
{
    public sealed class PaymentInitiateViewModel
    {
        public int AmountSats { get; set; }

        public string Description { get; set; } = "Membership";

        public Guid? SubscriptionId { get; set; }

        public Guid? PaymentRecordId { get; set; }

        public string? PaymentId { get; set; }

        public string? PaymentRequest { get; set; }

        public string? PaymentUrl { get; set; }

        public DateTimeOffset? ExpiresAt { get; set; }
    }
}
