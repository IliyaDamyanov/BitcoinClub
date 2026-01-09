using System;

namespace BitcoinClub.ViewModels
{
    public sealed class PaymentStatusViewModel
    {
        public DateTime ExpirationDate { get; set; }

        public DateTime? LastPaymentDate { get; set; }
    }
}
