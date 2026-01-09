using System.Collections.Generic;
using BitcoinClub.Infrastructure.Payments;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BitcoinClub.Tests.Payments
{
    public class BreezServiceRegistrationTests
    {
        [Fact]
        public void AddBreezPayments_RegistersFactory()
        {
            var cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string?>("Breez:ApiKey", "k")
                })
                .Build();

            var services = new ServiceCollection();
            services.AddBreezPayments(cfg);

            var sp = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
            var factory = sp.GetRequiredService<IBreezClientFactory>();

            Assert.NotNull(factory);
        }
    }
}
