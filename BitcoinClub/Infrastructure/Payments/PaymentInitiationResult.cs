using System;

namespace BitcoinClub.Infrastructure.Payments
{
    public sealed record PaymentInitiationResult(
        Guid SubscriptionId,
        string PaymentId,
        string? PaymentRequest,
        int AmountSats,
        string? PaymentUrl = null,
        DateTimeOffset? ExpiresAt = null);
}
