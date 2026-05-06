using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BitcoinClub.Infrastructure.Payments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BitcoinClub.Controllers
{
    [ApiController]
    [Route("webhooks/glow-pay")]
    public sealed class GlowPayWebhookController : ControllerBase
    {
        private const string Provider = "glow-pay";
        private readonly IPaymentService _payments;
        private readonly GlowPayOptions _options;
        private readonly ILogger<GlowPayWebhookController> _logger;

        public GlowPayWebhookController(
            IPaymentService payments,
            IOptions<GlowPayOptions> options,
            ILogger<GlowPayWebhookController> logger)
        {
            _payments = payments;
            _options = options.Value;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Receive()
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var body = await reader.ReadToEndAsync();
            var signature = Request.Headers["X-Glow-Signature"].ToString();

            if (!IsValidSignature(body, signature))
            {
                _logger.LogWarning("Rejected Glow Pay webhook with invalid signature.");
                return Unauthorized();
            }

            using var document = JsonDocument.Parse(body);
            var root = document.RootElement;
            var eventName = ReadString(root, "event") ?? ReadString(root, "type") ?? string.Empty;

            if (string.Equals(eventName, "payment.completed", StringComparison.OrdinalIgnoreCase))
            {
                return await HandleCompletedAsync(root);
            }

            if (string.Equals(eventName, "payment.created", StringComparison.OrdinalIgnoreCase)
                || string.Equals(eventName, "payment.expired", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Accepted Glow Pay webhook event {EventName} without mutation.", eventName);
                return Ok(new { received = true });
            }

            _logger.LogInformation("Accepted unhandled Glow Pay webhook event {EventName} without mutation.", eventName);
            return Ok(new { received = true });
        }

        private async Task<IActionResult> HandleCompletedAsync(JsonElement root)
        {
            var data = root.TryGetProperty("data", out var dataElement) ? dataElement : root;
            var paymentId = ReadString(data, "paymentId") ?? ReadString(data, "id") ?? string.Empty;
            var paidAt = ReadDateTimeOffset(data, "paidAt") ?? ReadDateTimeOffset(root, "createdAt");

            if (string.IsNullOrWhiteSpace(paymentId))
            {
                _logger.LogWarning("Glow Pay payment.completed webhook omitted payment id.");
                return BadRequest(new { error = "paymentId is required" });
            }

            var result = await _payments.CompleteProviderPaymentAsync(Provider, paymentId, paidAt);
            if (!result.IsPaid)
            {
                _logger.LogWarning("Glow Pay payment.completed webhook referenced unknown payment {PaymentId}.", paymentId);
                return NotFound(new { error = "payment not found" });
            }

            _logger.LogInformation("Completed Glow Pay payment {PaymentId} from webhook.", paymentId);
            return Ok(new { received = true });
        }

        private bool IsValidSignature(string body, string signature)
        {
            if (string.IsNullOrWhiteSpace(_options.WebhookSecret)
                || _options.WebhookSecret.StartsWith("YOUR_", StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrWhiteSpace(signature))
            {
                return false;
            }

            var received = signature.StartsWith("sha256=", StringComparison.OrdinalIgnoreCase)
                ? signature[7..]
                : signature;

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_options.WebhookSecret));
            var expectedBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(body));
            var expected = Convert.ToHexString(expectedBytes).ToLowerInvariant();

            return received.Length == expected.Length
                && CryptographicOperations.FixedTimeEquals(
                    Encoding.ASCII.GetBytes(received.ToLowerInvariant()),
                    Encoding.ASCII.GetBytes(expected));
        }

        private static string? ReadString(JsonElement element, string name)
        {
            if (!element.TryGetProperty(name, out var value) || value.ValueKind != JsonValueKind.String)
            {
                return null;
            }

            return value.GetString();
        }

        private static DateTimeOffset? ReadDateTimeOffset(JsonElement element, string name)
        {
            var value = ReadString(element, name);
            if (DateTimeOffset.TryParse(value, out var parsed))
            {
                return parsed;
            }

            return null;
        }
    }
}
