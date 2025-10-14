using FrameworkProject;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll.Microsoft.Extensions.DependencyInjection;
using Tests.Automation.Pages;

namespace Tests.Automation.Support;

public static class Dependencies
{
    [ScenarioDependencies]
    public static IServiceCollection ConfigureServices()
    {
        var services = new ServiceCollection();

        var cfg = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        services
            .AddSingleton<IConfig>(new Config
            {
                BaseUrl = cfg["BaseUrl"] ?? "http://localhost:8080", 
                SftpHost = cfg["SftpHost"] ?? "localhost",
                SftpPort = int.TryParse(cfg["SftpPort"], out var p) ? p : 2222,
                SftpUser = cfg["SftpUser"] ?? "testuser", 
                SftpPassword = cfg["SftpPassword"] ?? "password",
                SftpInboundPath = cfg["SftpInboundPath"] ?? "/upload/inbound"
            });
        services
            .AddTransient<ILoginPage, LoginPage>()
            .AddScoped<UiContext>()
            .AddAutomationFramework(new ConfigurationBuilder().Build());
        return services;
    }
}