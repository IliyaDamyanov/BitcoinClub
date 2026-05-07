using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BitcoinClub.Infrastructure.Payments
{
    public static class GlowPayServiceCollectionExtensions
    {
        public static IServiceCollection AddGlowPayPayments(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<GlowPayOptions>()
                .Bind(configuration.GetSection(GlowPayOptions.SectionName))
                .ValidateOnStart();

            services.AddHttpClient<ILightningApiClient, GlowPayApiClient>();
            services.AddScoped<IPaymentService, LightningPaymentService>();

            return services;
        }
    }
}
