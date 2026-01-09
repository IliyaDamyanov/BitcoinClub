using System.Collections.Generic;
using BitcoinClub.Infrastructure.Payments;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BitcoinClub.Tests.Payments
{
    public class BreezOptionsBindingTests
    {
        [Fact]
        public void BreezOptions_BindsFromConfiguration()
        {
            var cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string?>("Breez:ApiKey", "k"),
                    new KeyValuePair<string, string?>("Breez:Environment", "test"),
                    new KeyValuePair<string, string?>("Breez:DefaultNodeUrl", "https://example")
                })
                .Build();

            var opts = new BreezOptions();
            cfg.GetSection(BreezOptions.SectionName).Bind(opts);

            Assert.Equal("k", opts.ApiKey);
            Assert.Equal("test", opts.Environment);
            Assert.Equal("https://example", opts.DefaultNodeUrl);
        }
    }
}
