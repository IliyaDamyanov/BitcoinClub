using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BitcoinClub.Infrastructure.Payments;
using Microsoft.Extensions.Options;
using Xunit;

namespace BitcoinClub.Tests.Payments
{
    public class GlowPayApiClientTests
    {
        [Fact]
        public async Task CreateInvoiceAsync_PostsPaymentRequest_AndReturnsCheckoutData()
        {
            var handler = new RecordingHandler((request, body) =>
            {
                Assert.Equal(HttpMethod.Post, request.Method);
                Assert.Equal("https://glow-pay.co/api/payments", request.RequestUri?.ToString());
                Assert.True(request.Headers.TryGetValues("X-API-Key", out var values));
                Assert.Contains("test-key", values);
                Assert.Contains("\"amountSats\":1234", body);
                Assert.Contains("\"description\":\"Membership\"", body);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "success": true,
                      "data": {
                        "paymentId": "m2abc_xyz123",
                        "paymentUrl": "https://glow-pay.co/pay/m2abc_xyz123",
                        "invoice": "lnbc...",
                        "expiresAt": "2026-05-06T12:00:00Z",
                        "amountSats": 1234
                      }
                    }
                    """)
                };
            });

            var sut = CreateClient(handler);

            var result = await sut.CreateInvoiceAsync(1234, "Membership");

            Assert.Equal("m2abc_xyz123", result.PaymentHash);
            Assert.Equal("lnbc...", result.PaymentRequest);
            Assert.Equal("https://glow-pay.co/pay/m2abc_xyz123", result.PaymentUrl);
            Assert.Equal(DateTimeOffset.Parse("2026-05-06T12:00:00Z"), result.ExpiresAt);
        }

        [Fact]
        public async Task GetPaymentStatusAsync_MapsCompletedStatusToPaid()
        {
            var handler = new RecordingHandler((request, _) =>
            {
                Assert.Equal(HttpMethod.Get, request.Method);
                Assert.Equal("https://glow-pay.co/api/payments/m2abc_xyz123", request.RequestUri?.ToString());

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                    {
                      "success": true,
                      "data": {
                        "id": "m2abc_xyz123",
                        "status": "completed",
                        "paidAt": "2026-05-06T12:02:30Z"
                      }
                    }
                    """)
                };
            });

            var sut = CreateClient(handler);

            var result = await sut.GetPaymentStatusAsync("m2abc_xyz123");

            Assert.True(result.IsPaid);
            Assert.Equal(DateTimeOffset.Parse("2026-05-06T12:02:30Z"), result.PaidAt);
        }

        private static GlowPayApiClient CreateClient(HttpMessageHandler handler)
        {
            var http = new HttpClient(handler);
            var options = Options.Create(new GlowPayOptions
            {
                BaseUrl = "https://glow-pay.co",
                ApiKey = "test-key"
            });

            return new GlowPayApiClient(http, options);
        }

        private sealed class RecordingHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, string, HttpResponseMessage> _respond;

            public RecordingHandler(Func<HttpRequestMessage, string, HttpResponseMessage> respond)
            {
                _respond = respond;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var body = request.Content is null
                    ? string.Empty
                    : await request.Content.ReadAsStringAsync(cancellationToken);

                return _respond(request, body);
            }
        }
    }
}
