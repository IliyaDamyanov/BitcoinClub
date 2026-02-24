using System.Threading;
using System.Threading.Tasks;

namespace BitcoinClub.Infrastructure.Payments
{
    public interface ILightningApiClient
    {
        Task<LightningInvoiceResponse> CreateInvoiceAsync(
            int amountSats,
            string description,
            CancellationToken cancellationToken = default);

        Task<LightningPaymentStatusResponse> GetPaymentStatusAsync(
            string paymentHash,
            CancellationToken cancellationToken = default);
    }

    public sealed record LightningInvoiceResponse(string PaymentHash, string? PaymentRequest);

    public sealed record LightningPaymentStatusResponse(bool IsPaid, long? PaidAtUnixSeconds);
}
