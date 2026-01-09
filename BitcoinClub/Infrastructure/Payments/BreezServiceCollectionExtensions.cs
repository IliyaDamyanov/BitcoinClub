using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BitcoinClub.Infrastructure.Payments
{
    public static class BreezServiceCollectionExtensions
    {
        public static IServiceCollection AddBreezPayments(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<BreezOptions>()
                .Bind(configuration.GetSection(BreezOptions.SectionName))
                .Validate(o => !string.IsNullOrWhiteSpace(o.ApiKey), "Breez:ApiKey is required.")
                .ValidateOnStart();

            services.AddSingleton<IBreezClientFactory, BreezClientFactory>();

            return services;
        }
    }

    public interface IBreezClientFactory
    {
        object Create();
    }

    internal sealed class BreezClientFactory : IBreezClientFactory
    {
        private readonly IOptions<BreezOptions> _options;

        public BreezClientFactory(IOptions<BreezOptions> options)
        {
            _options = options;
        }

        public object Create()
        {
            _ = _options.Value;
            return new object();
        }
    }
}
