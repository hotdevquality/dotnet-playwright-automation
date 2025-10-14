using Microsoft.Playwright;

namespace FrameworkProject;

public interface IBrowserFactory
{
    Task<IBrowser> CreateBrowserAsync();
    Task<IBrowserContext> CreateContextAsync(IBrowser browser);
}

public class PlaywrightBrowserFactory : IBrowserFactory, IAsyncDisposable
{
    private readonly IConfig _cfg;
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public PlaywrightBrowserFactory(IConfig cfg) => _cfg = cfg;

    private async Task EnsureInitializedAsync()
    {
        if (_playwright is null)
            _playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        if (_browser is null)
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 50
            });
    }

    public async Task<IBrowser> CreateBrowserAsync()
    {
        await EnsureInitializedAsync();
        return _browser!;
    }

    public async Task<IBrowserContext> CreateContextAsync(IBrowser browser)
    {
        return await browser.NewContextAsync(new BrowserNewContextOptions
        {
            BaseURL = _cfg.BaseUrl,
            ViewportSize = new ViewportSize { Width = 1280, Height = 800 },
            RecordVideoDir = "artifacts/videos",
            RecordHarPath = "artifacts/network.har"
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser is not null)
        {
            await _browser.DisposeAsync(); 
            _browser = null;
        }
        _playwright?.Dispose();
        _playwright = null;
    }
}
