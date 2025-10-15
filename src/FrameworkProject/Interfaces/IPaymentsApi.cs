using System.Threading;
using System.Threading.Tasks;

namespace FrameworkProject
{
    public interface IPaymentsApi
    {
        Task<PaymentResponse> SubmitAsync(PaymentRequest request, CancellationToken ct = default);
        Task<PaymentResponse> SubmitAsync(PaymentRequest request, string? testCase, CancellationToken ct = default);
        Task<PaymentResponse> SubmitBankAsync(BankPayment bank, CancellationToken ct = default);
        Task<PaymentResponse> SubmitChequeAsync(ChequePayment cheque, CancellationToken ct = default);
        Task<PaymentResponse> SubmitCardAsync(CardPayment card, CancellationToken ct = default);
        Task<PaymentResponse> SubmitPaypalAsync(PaypalPayment paypal, CancellationToken ct = default);
    }
}
