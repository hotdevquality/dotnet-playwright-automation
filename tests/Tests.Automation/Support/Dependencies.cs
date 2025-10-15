using FrameworkProject;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll.Microsoft.Extensions.DependencyInjection;
using Tests.Automation.Pages;

namespace Tests.Automation.Support
{
    /// <summary>
    /// Configures dependency injection for Reqnroll scenarios.
    /// Reads configuration from appsettings.json and environment variables,
    /// then wires up the shared automation framework and test-level services.
    /// </summary>
    public static class Dependencies
    {
        [ScenarioDependencies]
        public static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            // ------------------------------------------------------
            // Build configuration root (shared by framework + tests)
            // ------------------------------------------------------
            var cfgRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Stagging"}.json",
                             optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            // ------------------------------------------------------
            // Register the core automation framework (shared DI)
            // ------------------------------------------------------
            services.AddAutomationFramework(cfgRoot);

            // ------------------------------------------------------
            // Register page objects, contexts, and any test-specific services
            // ------------------------------------------------------
            services
                .AddScoped<UiContext>()
                .AddTransient<ILoginPage, LoginPage>();

            // Add more page/service registrations as needed
            // .AddTransient<IHomePage, HomePage>()
            // .AddTransient<IPaymentPage, PaymentPage>()

            return services;
        }
    }
}
