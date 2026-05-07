using System;
using System.Threading;
using System.Threading.Tasks;

namespace BitcoinClub.Infrastructure.Payments
{
    public interface IPaymentService
    {
        Task<PaymentInitiationResult> InitiateMembershipPaymentAsync(
            string userId,
            int amountSats,
            string description,
            CancellationToken cancellationToken = default);

        Task<PaymentVerificationResult> VerifyPaymentAsync(
            Guid subscriptionId,
            string paymentId,
            CancellationToken cancellationToken = default);

        Task<PaymentVerificationResult> CompleteProviderPaymentAsync(
            string provider,
            string paymentId,
            DateTimeOffset? paidAt,
            CancellationToken cancellationToken = default);
    }
}
