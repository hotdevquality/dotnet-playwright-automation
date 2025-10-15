using System.Net.Http;
using Newtonsoft.Json;

namespace FrameworkProject;

public interface IStatusClient
{
    Task<string> GetAsync(string id, CancellationToken ct = default);
}

public class StatusClient : IStatusClient
{
    private readonly HttpClient _http;
    public StatusClient(HttpClient http) => _http = http;

    public async Task<string> GetAsync(string id, CancellationToken ct = default)
    {
        var resp = await _http.GetAsync($"/api/status/{id}", ct);
        resp.EnsureSuccessStatusCode();
        var json = await resp.Content.ReadAsStringAsync(ct);
        dynamic obj = JsonConvert.DeserializeObject(json)!;
        return (string)obj.status;
    }
}