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

            // For this task we expose a minimal Breez API abstraction. A real SDK-backed implementation can replace this.
            services.AddSingleton<IBreezApiClient, BreezApiClient>();
            services.AddScoped<IBreezePaymentService, BreezPaymentService>();

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

    internal sealed class BreezApiClient : IBreezApiClient
    {
        private readonly IBreezClientFactory _factory;

        public BreezApiClient(IBreezClientFactory factory)
        {
            _factory = factory;
        }

        public Task<BreezPaymentInitResponse> CreateInvoiceAsync(int amountSats, string description, CancellationToken cancellationToken = default)
        {
            _ = _factory.Create();
            // SDK integration is intentionally deferred; tests use mocks.
            return Task.FromResult(new BreezPaymentInitResponse(Guid.NewGuid().ToString("N"), null));
        }

        public Task<BreezPaymentStatusResponse> GetPaymentStatusAsync(string paymentId, CancellationToken cancellationToken = default)
        {
            _ = _factory.Create();
            return Task.FromResult(new BreezPaymentStatusResponse(IsPaid: false, PaidAtUnixSeconds: null));
        }
    }
}
