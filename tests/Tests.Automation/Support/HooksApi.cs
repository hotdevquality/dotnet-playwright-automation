using FrameworkProject;
using Reqnroll;

[Binding]
public class HooksApi
{
    private readonly IWireMockAdmin _admin;
    public HooksApi(IWireMockAdmin admin) => _admin = admin;

    [BeforeScenario("@api")]
    public async Task ResetWireMockScenarios() => await _admin.ResetScenariosAsync();
}
