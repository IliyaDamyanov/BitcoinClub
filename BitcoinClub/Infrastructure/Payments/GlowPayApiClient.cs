using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace BitcoinClub.Infrastructure.Payments
{
    public sealed class GlowPayApiClient : ILightningApiClient
    {
        private readonly HttpClient _http;
        private readonly GlowPayOptions _options;

        public GlowPayApiClient(HttpClient httpClient, IOptions<GlowPayOptions> options)
        {
            _options = options.Value;
            _http = httpClient;
            _http.BaseAddress = new Uri(_options.BaseUrl.TrimEnd('/') + "/");
        }

        public async Task<LightningInvoiceResponse> CreateInvoiceAsync(
            int amountSats,
            string description,
            CancellationToken cancellationToken = default)
        {
            EnsureApiKeyConfigured();

            var request = new GlowPayCreatePaymentRequest
            {
                AmountSats = amountSats,
                Description = description,
                Metadata = new Dictionary<string, string>
                {
                    ["source"] = "bitcoin-club",
                    ["purpose"] = "membership"
                }
            };

            using var message = new HttpRequestMessage(HttpMethod.Post, "api/payments")
            {
                Content = JsonContent.Create(request)
            };
            message.Headers.Add("X-API-Key", _options.ApiKey);

            var response = await _http.SendAsync(message, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<GlowPayResponse<GlowPayPaymentCreatedData>>(cancellationToken: cancellationToken)
                ?? throw new InvalidOperationException("Glow Pay returned null response.");

            if (!result.Success || result.Data is null)
            {
                throw new InvalidOperationException("Glow Pay did not return a successful payment creation response.");
            }

            return new LightningInvoiceResponse(
                result.Data.PaymentId,
                result.Data.Invoice,
                result.Data.PaymentUrl,
                result.Data.ExpiresAt);
        }

        public async Task<LightningPaymentStatusResponse> GetPaymentStatusAsync(
            string paymentHash,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(paymentHash))
                throw new ArgumentException("Payment id is required.", nameof(paymentHash));

            var response = await _http.GetAsync($"api/payments/{Uri.EscapeDataString(paymentHash)}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<GlowPayResponse<GlowPayPaymentStatusData>>(cancellationToken: cancellationToken)
                ?? throw new InvalidOperationException("Glow Pay returned null response.");

            if (!result.Success || result.Data is null)
            {
                throw new InvalidOperationException("Glow Pay did not return a successful payment status response.");
            }

            return new LightningPaymentStatusResponse(
                string.Equals(result.Data.Status, "completed", StringComparison.OrdinalIgnoreCase),
                null,
                result.Data.PaidAt);
        }

        private void EnsureApiKeyConfigured()
        {
            if (string.IsNullOrWhiteSpace(_options.ApiKey) || _options.ApiKey.StartsWith("YOUR_", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Glow Pay API key is not configured. Set GlowPay:ApiKey from the Glow Pay dashboard.");
            }
        }
    }

    internal sealed class GlowPayCreatePaymentRequest
    {
        [JsonPropertyName("amountSats")]
        public int AmountSats { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata { get; set; } = new();
    }

    internal sealed class GlowPayResponse<T>
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }

    internal sealed class GlowPayPaymentCreatedData
    {
        [JsonPropertyName("paymentId")]
        public string PaymentId { get; set; } = string.Empty;

        [JsonPropertyName("paymentUrl")]
        public string? PaymentUrl { get; set; }

        [JsonPropertyName("invoice")]
        public string? Invoice { get; set; }

        [JsonPropertyName("expiresAt")]
        public DateTimeOffset? ExpiresAt { get; set; }
    }

    internal sealed class GlowPayPaymentStatusData
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("paidAt")]
        public DateTimeOffset? PaidAt { get; set; }
    }
}
