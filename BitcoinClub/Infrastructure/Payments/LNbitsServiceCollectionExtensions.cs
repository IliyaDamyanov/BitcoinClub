using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BitcoinClub.Infrastructure.Payments
{
    public static class LNbitsServiceCollectionExtensions
    {
        public static IServiceCollection AddLNbitsPayments(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<LNbitsOptions>()
                .Bind(configuration.GetSection(LNbitsOptions.SectionName))
                .ValidateOnStart();

            services.AddHttpClient<ILightningApiClient, LNbitsApiClient>();

            services.AddScoped<IBreezePaymentService, LightningPaymentService>();

            return services;
        }
    }
}
