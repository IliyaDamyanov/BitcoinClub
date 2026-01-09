using System;
using Microsoft.AspNetCore.Identity;

namespace BitcoinClub.Models
{
    public class Subscription
    {
        public Guid Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public IdentityUser? User { get; set; }

        public DateTime ExpirationDate { get; set; }

        public DateTime? LastPaymentDate { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
