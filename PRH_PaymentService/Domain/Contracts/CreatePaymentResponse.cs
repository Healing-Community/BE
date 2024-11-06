namespace Domain.Contracts
{
    public class CreatePaymentResponse
    {
        public string PaymentUrl { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}