using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace FrameworkProject;

public interface IPaymentsApi
{
    Task<PaymentResponse> SubmitAsync(PaymentRequest request, CancellationToken ct = default);
    Task<PaymentResponse> SubmitAsync(PaymentRequest request, string? testCase, CancellationToken ct = default);
    Task<PaymentResponse> SubmitBankAsync(BankPayment bank, CancellationToken ct = default);
    Task<PaymentResponse> SubmitChequeAsync(ChequePayment cheque, CancellationToken ct = default);
    Task<PaymentResponse> SubmitCardAsync(CardPayment card, CancellationToken ct = default);
    Task<PaymentResponse> SubmitPaypalAsync(PaypalPayment paypal, CancellationToken ct = default);
}

public class PaymentsApi : IPaymentsApi
{
    private readonly HttpClient _http;
    public PaymentsApi(HttpClient http) => _http = http;

    public Task<PaymentResponse> SubmitBankAsync(BankPayment bank, CancellationToken ct = default) =>
        SubmitAsync(new PaymentRequest { Method = "bank", Bank = bank }, ct);
    
    public Task<PaymentResponse> SubmitChequeAsync(ChequePayment cheque, CancellationToken ct = default) =>
        SubmitAsync(new PaymentRequest { Method = "cheque", Cheque = cheque }, ct);

    public Task<PaymentResponse> SubmitCardAsync(CardPayment card, CancellationToken ct = default) =>
        SubmitAsync(new PaymentRequest { Method = "credit card", Card = card }, ct);

    public Task<PaymentResponse> SubmitPaypalAsync(PaypalPayment paypal, CancellationToken ct = default) =>
        SubmitAsync(new PaymentRequest { Method = "paypal", Paypal = paypal }, ct);

    public async Task<PaymentResponse> SubmitAsync(PaymentRequest request, CancellationToken ct = default)
    {
        var json = JsonConvert.SerializeObject(request);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var resp = await _http.PostAsync("/bank/submit", content, ct);
        resp.EnsureSuccessStatusCode();
        var body = await resp.Content.ReadAsStringAsync(ct);
        var parsed = JsonConvert.DeserializeObject<PaymentResponse>(body) ?? new PaymentResponse { Status = "unknown" };
        return parsed;
    }
    
    public async Task<PaymentResponse> SubmitAsync(PaymentRequest request, string? testCase, CancellationToken ct = default)
    {
        var url = "/bank/submit" + (string.IsNullOrEmpty(testCase) ? "" : $"?case={testCase}");
        var json = JsonConvert.SerializeObject(request);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var resp = await _http.PostAsync(url, content, ct);
        resp.EnsureSuccessStatusCode();                           // will throw for 400/422/503
        var body = await resp.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<PaymentResponse>(body) ?? new PaymentResponse { Status = "unknown" };
    }
    
    public async Task<(int Status, string Body)> SubmitRawAsync(PaymentRequest request, string? testCase, CancellationToken ct = default)
    {
        var url = "/bank/submit" + (string.IsNullOrEmpty(testCase) ? "" : $"?case={testCase}");
        var json = JsonConvert.SerializeObject(request);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var resp = await _http.PostAsync(url, content, ct);
        var body = await resp.Content.ReadAsStringAsync(ct);
        return ((int)resp.StatusCode, body);
    }
}

public class PaymentRequest
{
    public string Method { get; set; } = "";
    public BankPayment? Bank { get; set; }
    public ChequePayment? Cheque { get; set; }
    public CardPayment? Card { get; set; }
    public PaypalPayment? Paypal { get; set; }
}

public class BankPayment
{
    public string SortCode { get; set; } = "";
    public string AccountNumber { get; set; } = "";
    public string Recipient { get; set; } = "";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "GBP";
}

public class ChequePayment
{
    public string ChequeNumber { get; set; } = "";
    public string Recipient { get; set; } = "";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "GBP";
}

public class CardPayment
{
    public string CardNumber { get; set; } = "";
    public string Expiry { get; set; } = "";
    public string Recipient { get; set; } = "";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "GBP";
}

public class PaypalPayment
{
    public string Email { get; set; } = "";
    public string Recipient { get; set; } = "";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
}

public class PaymentResponse
{
    public string Status { get; set; } = "";
    public string? Id { get; set; }
}