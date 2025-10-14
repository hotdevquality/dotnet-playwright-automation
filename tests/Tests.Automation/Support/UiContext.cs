using Microsoft.Playwright;

namespace Tests.Automation.Support;

public class UiContext
{
    public IBrowser? Browser { get; set; }
    public IBrowserContext? Context { get; set; }
    public IPage? Page { get; set; }
}