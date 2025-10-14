using FrameworkProject;
using FluentAssertions;
using Reqnroll;

namespace Tests.Automation.Steps;

[Binding]
public class SoapBankSteps
{
    private readonly ISoapBankClient _soap;
    private string? _status;

    public SoapBankSteps(ISoapBankClient soap) => _soap = soap;

    [When(@"I request bank status for account ""(.*)"" by SOAP")]
    public async Task WhenIRequest(string account)
    {
        _status = await _soap.GetBankStatusAsync(account);
    }

    [Then(@"the SOAP status should be ""(.*)""")]
    public void ThenStatus(string expected) => _status.Should().Be(expected);
}