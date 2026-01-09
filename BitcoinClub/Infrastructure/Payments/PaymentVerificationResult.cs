using System;

namespace BitcoinClub.Infrastructure.Payments
{
    public sealed record PaymentVerificationResult(
        bool IsPaid,
        DateTimeOffset? PaidAt,
        DateTime? NewExpirationDate);
}
