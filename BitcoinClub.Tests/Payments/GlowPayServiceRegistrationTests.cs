using System.Collections.Generic;
using BitcoinClub.Infrastructure.Payments;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BitcoinClub.Tests.Payments
{
    public class GlowPayServiceRegistrationTests
    {
        [Fact]
        public void AddGlowPayPayments_RegistersLightningApiClient()
        {
            var cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string?>("GlowPay:BaseUrl", "https://glow-pay.co"),
                    new KeyValuePair<string, string?>("GlowPay:ApiKey", "test-key")
                })
                .Build();

            var services = new ServiceCollection();
            services.AddGlowPayPayments(cfg);

            var sp = services.BuildServiceProvider();
            var client = sp.GetService<ILightningApiClient>();

            Assert.NotNull(client);
            Assert.IsType<GlowPayApiClient>(client);
        }
    }
}
