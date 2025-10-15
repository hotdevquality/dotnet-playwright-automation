namespace FrameworkProject
{
    public class PaymentRequest
    {
        public string Method { get; set; } = "";
        public BankPayment? Bank { get; set; }
        public ChequePayment? Cheque { get; set; }
        public CardPayment? Card { get; set; }
        public PaypalPayment? Paypal { get; set; }
    }
}