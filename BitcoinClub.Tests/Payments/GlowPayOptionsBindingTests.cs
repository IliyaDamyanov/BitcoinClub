using System.Collections.Generic;
using BitcoinClub.Infrastructure.Payments;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BitcoinClub.Tests.Payments
{
    public class GlowPayOptionsBindingTests
    {
        [Fact]
        public void GlowPayOptions_BindsFromConfiguration()
        {
            var cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string?>("GlowPay:BaseUrl", "https://glow-pay.co"),
                    new KeyValuePair<string, string?>("GlowPay:ApiKey", "test-key"),
                    new KeyValuePair<string, string?>("GlowPay:WebhookSecret", "test-secret")
                })
                .Build();

            var opts = new GlowPayOptions();
            cfg.GetSection(GlowPayOptions.SectionName).Bind(opts);

            Assert.Equal("https://glow-pay.co", opts.BaseUrl);
            Assert.Equal("test-key", opts.ApiKey);
            Assert.Equal("test-secret", opts.WebhookSecret);
        }
    }
}
