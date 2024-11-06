namespace Domain.Contracts
{
    public class PaymentRequest
    {
        public string OrderCode { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}