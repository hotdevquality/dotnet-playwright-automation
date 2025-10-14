
namespace FrameworkProject;

public interface IWireMockAdmin
{
    Task ResetScenariosAsync(CancellationToken ct = default);
}

public class WireMockAdmin : IWireMockAdmin
{
    private readonly HttpClient _http;
    public WireMockAdmin(HttpClient http) => _http = http;

    public Task ResetScenariosAsync(CancellationToken ct = default)
        => _http.PostAsync("/__admin/scenarios/reset", content: null, ct);
}