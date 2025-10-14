using FrameworkProject;
using FluentAssertions;
using Microsoft.Playwright;
using Reqnroll;
using Tests.Automation.Pages;
using Tests.Automation.Support;

namespace Tests.Automation.Steps;

[Binding]
public class PaymentSteps
{
    private readonly IBrowserFactory _factory;
    private readonly ILoginPage _loginPage;
    private readonly UiContext _ui;

    public PaymentSteps(IBrowserFactory factory, ILoginPage loginPage, UiContext ui)
    {
        _factory = factory;
        _loginPage = loginPage;
        _ui = ui;
    }

    [Given(@"I log in as ""(.*)""")]
    public async Task GivenILogInAs(string role)
    {
        if (_ui.Context is null)
        {
            _ui.Browser = await _factory.CreateBrowserAsync();
            _ui.Context = await _factory.CreateContextAsync(_ui.Browser);
            _ui.Page = await _ui.Context.NewPageAsync();
        }

        await _loginPage.LoginAsync(_ui.Context!, role, "password");
    }

    [Given(@"I open the Create Payment page")]
    public async Task GivenIOpenCreatePayment()
    {
        await _ui.Page!.GotoAsync("/payments/create");
    }
    
    [Given(@"I choose payment method ""(.*)""")]
    public async Task GivenIChoosePaymentMethod(string method)
    {
        await _ui.Page!.SelectOptionAsync("select[name='method']", new[] { method.ToLowerInvariant() });
    }

    [Given(@"I enter cheque number ""(.*)""")]
    public async Task GivenIEnterChequeNumber(string chequeNo)
    {
        await _ui.Page!.FillAsync("input[name='chequeNumber']", chequeNo);
    }

    [Given(@"I enter card number ""(.*)"" and expiry ""(.*)""")]
    public async Task GivenIEnterCard(string card, string expiry)
    {
        await _ui.Page!.FillAsync("input[name='cardNumber']", card);
        await _ui.Page!.FillAsync("input[name='cardExpiry']", expiry);
    }

    [Given(@"I enter PayPal email ""(.*)""")]
    public async Task GivenIEnterPaypalEmail(string email)
    {
        await _ui.Page!.FillAsync("input[name='paypalEmail']", email);
    }

    [When(@"I submit a UK bank payment with sort code ""(.*)"" and account ""(.*)""")]
    public async Task WhenISubmit(string sortCode, string account)
    {
        var page = await _ui.Context!.NewPageAsync();
        await page.GotoAsync("/payments/create");
        await page.FillAsync("input[name='sortCode']", sortCode);
        await page.FillAsync("input[name='accountNumber']", account);
        await page.ClickAsync("button[type='submit']");
    }
    
    [When(@"I submit the payment")]
    public async Task WhenISubmitThePayment()
    {
        await _ui.Page!.ClickAsync("button[type='submit']");
    }

    [Then(@"the payment appears with status ""(.*)""")]
    public async Task ThenPaymentAppearsWithStatus(string status)
    {
        var page = await _ui.Context!.NewPageAsync();
        await page.GotoAsync("/payments/list");
        var cell = page.Locator("tbody tr:first-child td.status");
        await cell.WaitForAsync();
        (await cell.InnerTextAsync()).Should().Be(status);
    }
    
    [Then(@"I should be on the payments list")]
    public async Task ThenIShouldBeOnThePaymentsList()
    {
        await _ui.Page!.WaitForURLAsync("**/payments/list");
    }
}
