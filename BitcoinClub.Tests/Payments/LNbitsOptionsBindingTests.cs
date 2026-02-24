using System.Collections.Generic;
using BitcoinClub.Infrastructure.Payments;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BitcoinClub.Tests.Payments
{
    public class LNbitsOptionsBindingTests
    {
        [Fact]
        public void LNbitsOptions_BindsFromConfiguration()
        {
            var cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string?>("LNbits:BaseUrl", "https://my.lnbits.com"),
                    new KeyValuePair<string, string?>("LNbits:ApiKey", "test-key")
                })
                .Build();

            var opts = new LNbitsOptions();
            cfg.GetSection(LNbitsOptions.SectionName).Bind(opts);

            Assert.Equal("https://my.lnbits.com", opts.BaseUrl);
            Assert.Equal("test-key", opts.ApiKey);
        }
    }
}
