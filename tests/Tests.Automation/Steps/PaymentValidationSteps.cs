using Reqnroll;
using Tests.Automation.Support;

namespace Tests.Automation.Steps;

[Binding]
public class PaymentValidationSteps
{
    private readonly UiContext _ui;
    public PaymentValidationSteps(UiContext ui) => _ui = ui;

    [Given(@"I enter bank account number ""(.*)""")]
    public async Task GivenIEnterBankAccountNumber(string acct)
    {
        await _ui.Page!.FillAsync("input[name='accountNumber']", acct);
    }

    [Then(@"I should see a validation error ""(.*)""")]
    public async Task ThenSeeError(string message)
    {
        await _ui.Page!.WaitForSelectorAsync("text=" + message);
    }
}