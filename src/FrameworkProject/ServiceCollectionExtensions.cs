using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FrameworkProject;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAutomationFramework(this IServiceCollection services, IConfiguration cfg)
    {
        services
            .AddSingleton<IConfig, Config>()
            .AddSingleton<IBrowserFactory, PlaywrightBrowserFactory>()
            .AddTransient<IBatchFileBuilder, BatchFileBuilder>()
            .AddTransient<ISftpClient, SshNetSftpClient>()
            .AddTransient<IPaymentsApi, PaymentsApi>();
        
        services.AddHttpClient<IPaymentsApi, PaymentsApi>((sp, client) =>
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
