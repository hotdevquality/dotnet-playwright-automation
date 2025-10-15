namespace FrameworkProject
{
    public class PaypalPayment
    {
        public string Email { get; set; } = "";
        public string Recipient { get; set; } = "";
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EUR";
    }
}