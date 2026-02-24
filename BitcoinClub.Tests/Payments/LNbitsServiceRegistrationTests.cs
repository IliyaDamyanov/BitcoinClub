using System.Collections.Generic;
using BitcoinClub.Infrastructure.Payments;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BitcoinClub.Tests.Payments
{
    public class LNbitsServiceRegistrationTests
    {
        [Fact]
        public void AddLNbitsPayments_RegistersLightningApiClient()
        {
            var cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string?>("LNbits:BaseUrl", "https://legend.lnbits.com"),
                    new KeyValuePair<string, string?>("LNbits:ApiKey", "test-key")
                })
                .Build();

            var services = new ServiceCollection();
            services.AddLNbitsPayments(cfg);

            var sp = services.BuildServiceProvider();
            var client = sp.GetService<ILightningApiClient>();

            Assert.NotNull(client);
        }
    }
}
