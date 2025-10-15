namespace FrameworkProject
{
    public class CardPayment
    {
        public string CardNumber { get; set; } = "";
        public string Expiry { get; set; } = "";
        public string Recipient { get; set; } = "";
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "GBP";
    }
}