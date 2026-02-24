using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace BitcoinClub.Infrastructure.Payments
{
    public sealed class LNbitsApiClient : ILightningApiClient
    {
        private readonly HttpClient _http;

        public LNbitsApiClient(HttpClient httpClient, IOptions<LNbitsOptions> options)
        {
            _http = httpClient;
            _http.BaseAddress = new Uri(options.Value.BaseUrl.TrimEnd('/') + "/");
            _http.DefaultRequestHeaders.Add("X-Api-Key", options.Value.ApiKey);
        }

        public async Task<LightningInvoiceResponse> CreateInvoiceAsync(
            int amountSats,
            string description,
            CancellationToken cancellationToken = default)
        {
            var request = new LNbitsCreateInvoiceRequest
            {
                Out = false,
                Amount = amountSats,
                Memo = description
            };

            var response = await _http.PostAsJsonAsync("api/v1/payments", request, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<LNbitsInvoiceResult>(cancellationToken: cancellationToken)
                ?? throw new InvalidOperationException("LNbits returned null response.");

            return new LightningInvoiceResponse(result.PaymentHash, result.PaymentRequest);
        }

        public async Task<LightningPaymentStatusResponse> GetPaymentStatusAsync(
            string paymentHash,
            CancellationToken cancellationToken = default)
        {
            var response = await _http.GetAsync($"api/v1/payments/{paymentHash}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<LNbitsPaymentStatusResult>(cancellationToken: cancellationToken)
                ?? throw new InvalidOperationException("LNbits returned null response.");

            return new LightningPaymentStatusResponse(result.Paid, result.Details?.Time);
        }
    }

    internal sealed class LNbitsCreateInvoiceRequest
    {
        [JsonPropertyName("out")]
        public bool Out { get; set; }

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("memo")]
        public string Memo { get; set; } = string.Empty;
    }

    internal sealed class LNbitsInvoiceResult
    {
        [JsonPropertyName("payment_hash")]
        public string PaymentHash { get; set; } = string.Empty;

        [JsonPropertyName("payment_request")]
        public string PaymentRequest { get; set; } = string.Empty;
    }

    internal sealed class LNbitsPaymentStatusResult
    {
        [JsonPropertyName("paid")]
        public bool Paid { get; set; }

        [JsonPropertyName("details")]
        public LNbitsPaymentDetails? Details { get; set; }
    }

    internal sealed class LNbitsPaymentDetails
    {
        [JsonPropertyName("time")]
        public long? Time { get; set; }
    }
}
