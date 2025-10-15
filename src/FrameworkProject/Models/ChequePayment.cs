namespace FrameworkProject
{
    public class ChequePayment
    {
        public string ChequeNumber { get; set; } = "";
        public string Recipient { get; set; } = "";
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "GBP";
    }
}