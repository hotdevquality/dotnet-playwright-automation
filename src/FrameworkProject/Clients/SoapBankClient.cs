using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace FrameworkProject;

public interface ISoapBankClient
{
    Task<string> GetBankStatusAsync(string accountNumber, CancellationToken ct = default);
}

public class SoapBankClient : ISoapBankClient
{
    private readonly HttpClient _http;
    public SoapBankClient(HttpClient http) => _http = http;

    public async Task<string> GetBankStatusAsync(string accountNumber, CancellationToken ct = default)
    {
        var envelope = $@"<?xml version=""1.0"" encoding=""utf-8""?>
            <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ns=""http://example.com/bank"">
                <soap:Body>
                    <ns:GetBankStatusRequest>
                    <ns:AccountNumber>{System.Security.SecurityElement.Escape(accountNumber)}</ns:AccountNumber>
                    </ns:GetBankStatusRequest>
                </soap:Body>
            </soap:Envelope>";

        using var req = new HttpRequestMessage(HttpMethod.Post, "/soap/bank");
        req.Content = new StringContent(envelope, Encoding.UTF8, "text/xml");
        req.Headers.Add("SOAPAction", "http://example.com/bank/GetBankStatus");

        var resp = await _http.SendAsync(req, ct);
        var xml = await resp.Content.ReadAsStringAsync(ct);

        // If HTTP error, include body for debugging
        if (!resp.IsSuccessStatusCode)
            throw new HttpRequestException($"SOAP HTTP {(int)resp.StatusCode}: {xml}");

        var x = XDocument.Parse(xml);

        // Detect SOAP 1.1 Fault
        XNamespace soap11 = "http://schemas.xmlsoap.org/soap/envelope/";
        var fault = x.Descendants(soap11 + "Fault").FirstOrDefault();
        if (fault is not null)
        {
            var reason = fault.Element("faultstring")?.Value ?? "Unknown SOAP fault";
            throw new InvalidOperationException($"SOAP Fault: {reason}");
        }

        // Happy path
        XNamespace ns = "http://example.com/bank";
        return x.Descendants(ns + "Status").First().Value;
    }
}