using FrameworkProject;
using Reqnroll;

namespace Tests.Automation.Support;

[Binding]
public class Hooks
{
    private readonly IBrowserFactory _factory;
    private readonly ScenarioContext _scenarioContext;
    private readonly UiContext _ui;

    public Hooks(IBrowserFactory factory, ScenarioContext scenarioContext, UiContext ui)
    {
        _factory = factory;
        _scenarioContext = scenarioContext;
        _ui = ui;
    }

    [BeforeScenario("@ui")]
    public async Task BeforeScenario()
    {
        _ui.Browser = await _factory.CreateBrowserAsync();
        _ui.Context = await _factory.CreateContextAsync(_ui.Browser);
        _ui.Page = await _ui.Context.NewPageAsync();

        await _ui.Context.Tracing.StartAsync(new()
        {
            Title = _scenarioContext.ScenarioInfo.Title,
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    [AfterScenario("@ui")]
    public async Task AfterScenario()
    {
        string? tracePath = null;

        try
        {
            var safe = string.Concat(_scenarioContext.ScenarioInfo.Title.Split(Path.GetInvalidFileNameChars()))
                .Replace(' ', '_');
            tracePath = Path.Combine("artifacts", $"trace_{DateTime.UtcNow:yyyyMMdd_HHmmss}_{safe}.zip");
            await _ui.Context!.Tracing.StopAsync(new() { Path = tracePath });
        }
        catch
        {
            // ignore tracing stop errors
        }

        // ✅ Remove all AllureApi.AddAttachment calls — Allure.Reqnroll will handle reporting.
        // If you still want to preserve artifacts locally (for debugging), keep this block only for housekeeping.

        if (_ui.Page is not null)
            await _ui.Page.CloseAsync();

        if (_ui.Context is not null)
            await _ui.Context.CloseAsync();

        _ui.Page = null;
        _ui.Context = null;
    }
    
    
}