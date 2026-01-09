using System.Threading;
using System.Threading.Tasks;

namespace BitcoinClub.Infrastructure.Payments
{
    public interface IBreezApiClient
    {
        Task<BreezPaymentInitResponse> CreateInvoiceAsync(
            int amountSats,
            string description,
            CancellationToken cancellationToken = default);

        Task<BreezPaymentStatusResponse> GetPaymentStatusAsync(
            string paymentId,
            CancellationToken cancellationToken = default);
    }

    public sealed record BreezPaymentInitResponse(string PaymentId, string? PaymentRequest);

    public sealed record BreezPaymentStatusResponse(bool IsPaid, long? PaidAtUnixSeconds);
}


