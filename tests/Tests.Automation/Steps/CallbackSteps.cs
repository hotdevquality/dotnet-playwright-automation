using System.Diagnostics;
using FrameworkProject;
using FluentAssertions;
using Reqnroll;

namespace Tests.Automation.Steps;

[Binding]
public class CallbackSteps
{
    private readonly IStatusClient _status;
    private readonly ScenarioContext _ctx;

    public CallbackSteps(IStatusClient status, ScenarioContext ctx)
    {
        _status = status;
        _ctx = ctx;
    }

    [When(@"I poll the status for the returned id until ""(.*)"" or timeout (.*)s")]
    public async Task WhenIPollUntil(string expected, int seconds)
    {
        var resp = (PaymentResponse)_ctx["lastResponse"];
        resp.Id.Should().NotBeNullOrWhiteSpace();
        var sw = Stopwatch.StartNew();
        var delay = TimeSpan.FromMilliseconds(250);

        string current = "";
        while (sw.Elapsed < TimeSpan.FromSeconds(seconds))
        {
            current = await _status.GetAsync(resp.Id!);
            if (string.Equals(current, expected, StringComparison.OrdinalIgnoreCase))
                return;
            await Task.Delay(delay);
        }

        throw new TimeoutException($"Expected status '{expected}' but last status was '{current}'.");
    }
}