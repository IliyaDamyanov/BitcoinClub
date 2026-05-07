using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BitcoinClub.Controllers;
using BitcoinClub.Infrastructure.Payments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace BitcoinClub.Tests.Payments
{
    public class GlowPayWebhookControllerTests
    {
        [Fact]
        public async Task Receive_InvalidSignature_ReturnsUnauthorized()
        {
            var service = new FakePaymentService();
            var controller = CreateController(service, "secret", "{\"event\":\"payment.completed\"}", "bad");

            var result = await controller.Receive();

            Assert.IsType<UnauthorizedResult>(result);
            Assert.Equal(0, service.CompleteCalls);
        }

        [Fact]
        public async Task Receive_PaymentCompleted_WithValidSignature_CompletesPayment()
        {
            var body = "{\"event\":\"payment.completed\",\"data\":{\"paymentId\":\"pay-1\",\"paidAt\":\"2026-05-06T11:40:00Z\"}}";
            var signature = Sign(body, "secret");
            var service = new FakePaymentService { CompletionResult = new PaymentVerificationResult(true, DateTimeOffset.UtcNow, DateTime.UtcNow.AddMonths(1)) };
            var controller = CreateController(service, "secret", body, signature);

            var result = await controller.Receive();

            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(1, service.CompleteCalls);
            Assert.Equal("glow-pay", service.Provider);
            Assert.Equal("pay-1", service.PaymentId);
            Assert.NotNull(service.PaidAt);
        }

        [Fact]
        public async Task Receive_PaymentCreated_WithValidSignature_DoesNotMutatePayment()
        {
            var body = "{\"event\":\"payment.created\",\"data\":{\"paymentId\":\"pay-1\"}}";
            var signature = Sign(body, "secret");
            var service = new FakePaymentService();
            var controller = CreateController(service, "secret", body, signature);

            var result = await controller.Receive();

            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(0, service.CompleteCalls);
        }

        private static GlowPayWebhookController CreateController(
            FakePaymentService service,
            string secret,
            string body,
            string signature)
        {
            var options = Options.Create(new GlowPayOptions { WebhookSecret = secret });
            var controller = new GlowPayWebhookController(service, options, NullLogger<GlowPayWebhookController>.Instance);
            var context = new DefaultHttpContext();
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
            context.Request.Headers["X-Glow-Signature"] = signature;
            controller.ControllerContext = new ControllerContext { HttpContext = context };
            return controller;
        }

        private static string Sign(string body, string secret)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            return Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(body))).ToLowerInvariant();
        }

        private sealed class FakePaymentService : IPaymentService
        {
            public int CompleteCalls { get; private set; }
            public string? Provider { get; private set; }
            public string? PaymentId { get; private set; }
            public DateTimeOffset? PaidAt { get; private set; }
            public PaymentVerificationResult CompletionResult { get; set; } = new(false, null, null);

            public Task<PaymentInitiationResult> InitiateMembershipPaymentAsync(string userId, int amountSats, string description, CancellationToken cancellationToken = default)
                => throw new NotSupportedException();

            public Task<PaymentVerificationResult> VerifyPaymentAsync(Guid subscriptionId, string paymentId, CancellationToken cancellationToken = default)
                => throw new NotSupportedException();

            public Task<PaymentVerificationResult> CompleteProviderPaymentAsync(string provider, string paymentId, DateTimeOffset? paidAt, CancellationToken cancellationToken = default)
            {
                CompleteCalls++;
                Provider = provider;
                PaymentId = paymentId;
                PaidAt = paidAt;
                return Task.FromResult(CompletionResult);
            }
        }
    }
}
