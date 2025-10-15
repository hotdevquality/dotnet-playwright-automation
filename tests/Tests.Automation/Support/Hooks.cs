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

    // --------------------------------------------------------------------
    // BEFORE SCENARIO (UI)
    // - Creates Browser/Context/Page
    // - Starts Playwright tracing (screenshots/snapshots/sources)
    // - Ensures artifact folders exist
    // --------------------------------------------------------------------
    [BeforeScenario("@ui")]
    public async Task BeforeScenario()
    {
        Directory.CreateDirectory("artifacts");
        Directory.CreateDirectory("artifacts/screens");
        Directory.CreateDirectory("artifacts/videos");

        _ui.Browser = await _factory.CreateBrowserAsync();
        _ui.Context = await _factory.CreateContextAsync(_ui.Browser); // make sure factory sets RecordVideoDir
        _ui.Page = await _ui.Context.NewPageAsync();

        await _ui.Context.Tracing.StartAsync(new()
        {
            Title = _scenarioContext.ScenarioInfo.Title,
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    // --------------------------------------------------------------------
    // AFTER STEP
    // - On step failure, take a full-page screenshot
    // - Attach it to the report if the formatter supports attachments
    // --------------------------------------------------------------------
    [AfterStep]
    public async Task AfterStepAsync()
    {
        if (_scenarioContext.TestError == null) return;
        if (_ui.Page is null) return;

        try
        {
            var safeScenario = string.Concat(
                _scenarioContext.ScenarioInfo.Title.Select(c =>
                    Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));

            var safeStep = string.Concat(
                _scenarioContext.StepContext.StepInfo.Text.Select(c =>
                    Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));

            var shotPath = Path.Combine("artifacts", "screens",
                $"{safeScenario}_{safeStep}.png");

            await _ui.Page.ScreenshotAsync(new() { Path = shotPath, FullPage = true });

            TryAttach(shotPath, "image/png", "Failure screenshot");
        }
        catch
        {
            // ignore screenshot/attach issues so we don't mask real test failures
        }
    }

    // --------------------------------------------------------------------
    // AFTER SCENARIO (UI)
    // - Stops trace and saves ZIP (attaches if possible)
    // - Resolves and attaches page video if Playwright recorded it
    // - Closes Page/Context and clears UiContext
    // --------------------------------------------------------------------
    [AfterScenario("@ui")]
    public async Task AfterScenario()
    {
        string? tracePath = null;
        string? videoPath = null;

        // Stop trace -> ZIP
        try
        {
            if (_ui.Context != null)
            {
                var safe = string.Concat(
                    _scenarioContext.ScenarioInfo.Title.Select(c =>
                        Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));

                tracePath = Path.Combine("artifacts",
                    $"trace_{System.DateTime.UtcNow:yyyyMMdd_HHmmss}_{safe}.zip");

                await _ui.Context.Tracing.StopAsync(new() { Path = tracePath });

                if (File.Exists(tracePath))
                    TryAttach(tracePath, "application/zip", "Playwright trace");
            }
        }
        catch
        {
            // ignore tracing stop errors
        }

        // Resolve page video (if RecordVideoDir was set when creating the context)
        try
        {
            if (_ui.Page?.Video != null)
            {
                var v = await _ui.Page.Video.PathAsync();
                if (!string.IsNullOrEmpty(v) && File.Exists(v))
                {
                    videoPath = v;
                    TryAttach(videoPath, "video/webm", "Run video");
                }
            }
        }
        catch
        {
            // ignore video resolution errors
        }

        // Clean-up Playwright
        try { if (_ui.Page is not null) await _ui.Page.CloseAsync(); } catch { }
        try { if (_ui.Context is not null) await _ui.Context.CloseAsync(); } catch { }

        _ui.Page = null;
        _ui.Context = null;
    }

    // --------------------------------------------------------------------
    // Helper: Try to attach a file to the scenario using Reqnroll's attachment API
    // without taking a hard dependency (we use reflection so this compiles even
    // if the method is not available in the installed formatter/runtime).
    // --------------------------------------------------------------------
    private void TryAttach(string path, string mimeType, string? name = null)
    {
        try
        {
            if (!File.Exists(path)) return;

            var method = typeof(ScenarioContext).GetMethod(
                "AddAttachment",
                new[] { typeof(string), typeof(string), typeof(string) });

            if (method != null)
            {
                method.Invoke(_scenarioContext, new object?[]
                {
                    path, mimeType, name ?? Path.GetFileName(path)
                });
            }
            // If there's no AddAttachment method in this runtime/formatter,
            // we silently skip — the HTML plugin may not support it.
        }
        catch
        {
            // Swallow — attachments must never break the test flow.
        }
    }
}
