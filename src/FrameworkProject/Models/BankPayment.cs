namespace FrameworkProject
{
    public class BankPayment
    {
        public string SortCode { get; set; } = "";
        public string AccountNumber { get; set; } = "";
        public string Recipient { get; set; } = "";
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "GBP";
    }
}