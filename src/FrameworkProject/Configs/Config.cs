using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace FrameworkProject
{
    public interface IConfig
    {
        string BaseUrl { get; }
        string SftpHost { get; }
        int    SftpPort { get; }
        string SftpUser { get; }
        string SftpPassword { get; }
        string SftpInboundPath { get; }

        /// <summary>
        /// Access nested configuration sections (e.g. "Playwright", "EPPlus").
        /// </summary>
        IConfigurationSection GetSection(string key);
    }

    /// <summary>
    /// Loads configuration from appsettings.json (+ environment-specific file + env vars),
    /// exposes strongly-typed top-level values, and allows hierarchical access.
    /// </summary>
    public class Config : IConfig
    {
        private readonly IConfigurationRoot _configuration;

        public string BaseUrl         { get; }
        public string SftpHost        { get; }
        public int    SftpPort        { get; }
        public string SftpUser        { get; }
        public string SftpPassword    { get; }
        public string SftpInboundPath { get; }

        public Config()
        {
            // Resolve a sensible base path for both IDE and test runner contexts
            // Prefer current dir; fallback to AppContext.BaseDirectory if needed.
            var basePath = Directory.GetCurrentDirectory();
            if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
            {
                basePath = AppContext.BaseDirectory;
            }

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();

            BaseUrl         = _configuration["BaseUrl"]        ?? "http://localhost:8080";
            SftpHost        = _configuration["SftpHost"]       ?? "localhost";
            SftpPort        = int.TryParse(_configuration["SftpPort"], out var port) ? port : 2222;
            SftpUser        = _configuration["SftpUser"]       ?? "testuser";
            SftpPassword    = _configuration["SftpPassword"]   ?? "password";
            SftpInboundPath = _configuration["SftpInboundPath"]?? "/upload/inbound";
        }

        public IConfigurationSection GetSection(string key) => _configuration.GetSection(key);
    }
}
