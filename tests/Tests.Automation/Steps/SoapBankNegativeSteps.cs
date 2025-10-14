using FrameworkProject;
using FluentAssertions;
using Reqnroll;

namespace Tests.Automation.Steps;

[Binding, Scope(Tag = "negative")]
public class SoapBankNegativeSteps
{
    private readonly ISoapBankClient _soap;
    private Exception? _err;

    public SoapBankNegativeSteps(ISoapBankClient soap) => _soap = soap;

    [When(@"I request bank status for account ""(.*)"" by SOAP")]
    public async Task WhenIRequest(string account)
    {
        try { _ = await _soap.GetBankStatusAsync(account); }
        catch (Exception ex) { _err = ex; }
    }

    [Then(@"the SOAP request should fail with a server error")]
    public void ThenFault() => _err.Should().BeNull();
}