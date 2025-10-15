using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FrameworkProject
{
    public class PaymentsApiClient : IPaymentsApi
    {
        private readonly HttpClient _http;
        public PaymentsApiClient(HttpClient http) => _http = http;

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
            return JsonConvert.DeserializeObject<PaymentResponse>(body) ?? new PaymentResponse { Status = "unknown" };
        }

        public async Task<PaymentResponse> SubmitAsync(PaymentRequest request, string? testCase, CancellationToken ct = default)
        {
            var url = "/bank/submit" + (string.IsNullOrEmpty(testCase) ? "" : $"?case={testCase}");
            var json = JsonConvert.SerializeObject(request);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync(url, content, ct);
            resp.EnsureSuccessStatusCode(); // throws for 4xx/5xx
            var body = await resp.Content.ReadAsStringAsync(ct);
            return JsonConvert.DeserializeObject<PaymentResponse>(body) ?? new PaymentResponse { Status = "unknown" };
        }

        // Optional helper if you need raw status/body in negative tests
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
}