using Microsoft.Extensions.Options;
using Microsoft.Playwright;

namespace FrameworkProject
{
    public interface IBrowserFactory
    {
        Task<IBrowser> CreateBrowserAsync();
        Task<IBrowserContext> CreateContextAsync(IBrowser browser);
    }

    public class PlaywrightBrowserFactory : IBrowserFactory, IAsyncDisposable
    {
        private readonly IConfig _cfg;
        private readonly PlaywrightSettings _settings;

        private IPlaywright? _playwright;
        private IBrowser? _browser;

        public PlaywrightBrowserFactory(IConfig cfg, IOptions<PlaywrightSettings> options)
        {
            _cfg = cfg;
            _settings = options.Value ?? new PlaywrightSettings();
        }

        private async Task EnsureInitializedAsync()
        {
            if (_playwright is null)
                _playwright = await Playwright.CreateAsync();

            if (_browser is null)
            {
                IBrowserType type = (_settings.Browser ?? "chromium").ToLowerInvariant() switch
                {
                    "firefox" => _playwright!.Firefox,
                    "webkit"  => _playwright!.Webkit,
                    _         => _playwright!.Chromium
                };

                _browser = await type.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = _settings.Headless,
                    SlowMo   = _settings.SlowMo
                });
            }
        }

        public async Task<IBrowser> CreateBrowserAsync()
        {
            await EnsureInitializedAsync();
            return _browser!;
        }

        public async Task<IBrowserContext> CreateContextAsync(IBrowser browser)
        {
            // Ensure artifact folders exist for attachments/videos/HAR
            Directory.CreateDirectory("artifacts");
            if (_settings.RecordVideo && !string.IsNullOrWhiteSpace(_settings.VideoDir))
                Directory.CreateDirectory(_settings.VideoDir);

            var options = new BrowserNewContextOptions
            {
                BaseURL        = _cfg.BaseUrl,
                ViewportSize   = new ViewportSize { Width = _settings.ViewportWidth, Height = _settings.ViewportHeight },
                AcceptDownloads = true,
                RecordVideoDir = _settings.RecordVideo ? _settings.VideoDir : null
            };

            if (_settings.RecordHar)
            {
                options.RecordHarPath = _settings.HarPath;
                #if !NET7_0
                options.RecordHarMode = HarMode.Minimal;
                #endif
            }

            return await browser.NewContextAsync(options);
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                if (_browser is not null)
                {
                    await _browser.DisposeAsync();
                    _browser = null;
                }
            }
            finally
            {
                _playwright?.Dispose();
                _playwright = null;
            }
        }
    }
}
