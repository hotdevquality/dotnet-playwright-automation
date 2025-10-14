using FrameworkProject;
using FluentAssertions;
using Reqnroll;

namespace Tests.Automation.Steps;

[Binding]
public class PaymentsApiSteps
{
    private readonly IPaymentsApi _api;
    private readonly ScenarioContext _scenarioContext;

    private BankPayment? _bank;
    private PaymentResponse? _response;

    public PaymentsApiSteps(IPaymentsApi api, ScenarioContext scenarioContext)
    {
        _api = api;
        _scenarioContext = scenarioContext;
    }

    [Given(@"a bank payment ""(.*)"" ""(.*)"" ""(.*)"" (.*) ""(.*)""")]
    public void GivenABankPayment(string recipient, string sort, string account, decimal amount, string ccy)
    {
        _bank = new BankPayment
        {
            Recipient = recipient,
            SortCode = sort,
            AccountNumber = account,
            Amount = amount,
            Currency = ccy
        };
    }

    [When(@"I submit the payment via API")]
    public async Task WhenISubmitPayment()
    {
        _response = await _api.SubmitBankAsync(_bank!);
        _scenarioContext["lastResponse"] = _response;
    }

    [Then(@"the API response status should be ""(.*)""")]
    public void ThenStatus(string expected)
    {
        _response!.Status.Should().Be(expected);
        _response.Id.Should().NotBeNullOrWhiteSpace();
    }
}