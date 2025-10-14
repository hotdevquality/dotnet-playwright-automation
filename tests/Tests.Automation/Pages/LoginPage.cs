using Microsoft.Playwright;

namespace Tests.Automation.Pages;

public interface ILoginPage { Task LoginAsync(IBrowserContext context, string user, string pass); }

public class LoginPage : ILoginPage
{
    public async Task LoginAsync(IBrowserContext context, string user, string pass)
    {
        var page = await context.NewPageAsync();
        await page.GotoAsync("/login");
        await page.FillAsync("input[name='username']", user);
        await page.FillAsync("input[name='password']", pass);
        await page.ClickAsync("button[type='submit']");
        await page.WaitForURLAsync("**/dashboard");
    }
}
