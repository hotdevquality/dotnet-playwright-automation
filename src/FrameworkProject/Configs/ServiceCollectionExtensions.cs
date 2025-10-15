using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FrameworkProject
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutomationFramework(this IServiceCollection services, IConfiguration cfg)
        {
            // Config + Settings
            services.AddSingleton<IConfig, Config>();
            services.Configure<PlaywrightSettings>(cfg.GetSection("Playwright"));

            // Core Framework Services
            services.AddSingleton<IBrowserFactory, PlaywrightBrowserFactory>();
            services.AddTransient<IBatchFileBuilder, BatchFileBuilder>();
            services.AddTransient<ISftpClient, SshNetSftpClient>();
            services.AddTransient<IPaymentsApi, PaymentsApiClient>();

            // HTTP Clients
            services.AddHttpClient<IPaymentsApi, PaymentsApiClient>((sp, client) =>
            {
                var c = sp.GetRequiredService<IConfig>();
                client.BaseAddress = new Uri(c.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            });

            services.AddHttpClient<ISoapBankClient, SoapBankClient>((sp, client) =>
            {
                var c = sp.GetRequiredService<IConfig>();
                client.BaseAddress = new Uri(c.BaseUrl);
            });

            services.AddHttpClient<IStatusClient, StatusClient>((sp, client) =>
            {
                var c = sp.GetRequiredService<IConfig>();
                client.BaseAddress = new Uri(c.BaseUrl);
            });

            services.AddHttpClient<IWireMockAdmin, WireMockAdmin>((sp, client) =>
            {
                var c = sp.GetRequiredService<IConfig>();
                client.BaseAddress = new Uri(c.BaseUrl);
            });

            return services;
        }
    }
}