using FrameworkProject;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Reqnroll;

namespace Tests.Automation.Steps;

[Binding, Scope(Tag = "negative")]
public class PaymentsApiNegativeSteps
{
    private readonly IPaymentsApi _api;
    private PaymentRequest? _req;
    private int _status;
    private string _body = "";

    public PaymentsApiNegativeSteps(IPaymentsApi api) => _api = api;

    [Given(@"a payment request with method ""(.*)""")]
    public void GivenPaymentWithMethod(string method)
    {
        _req = new PaymentRequest { Method = method, Bank = new BankPayment { Recipient = "X", SortCode = "10-11-12", AccountNumber = "12345678", Amount = 1, Currency = "GBP" } };
    }

    [Given(@"a bank payment ""(.*)"" ""(.*)"" ""(.*)"" (.*) ""(.*)""")]
    public void GivenBankPayment(string recipient, string sort, string account, decimal amount, string ccy)
    {
        _req = new PaymentRequest { Method = "bank", Bank = new BankPayment { Recipient = recipient, SortCode = sort, AccountNumber = account, Amount = amount, Currency = ccy } };
    }

    [When(@"I submit the payment via API with case ""(.*)""")]
    public async Task WhenSubmitWithCase(string @case)
    {
        // raw to capture status/body (no exception path)
        var concrete = (PaymentsApiClient)_api; // safe in this test suite
        (_status, _body) = await concrete.SubmitRawAsync(_req!, @case);
    }

    [Then(@"the API should return status code (.*) and error ""(.*)""")]
    public void ThenAssertError(int expected, string error)
    {
        _status.Should().Be(expected);
        var j = JObject.Parse(_body);
        j.Value<string>("error").Should().Be(error);
    }
}