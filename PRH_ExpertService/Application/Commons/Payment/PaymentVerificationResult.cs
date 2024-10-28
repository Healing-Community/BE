namespace Application.Commons.Payment
{
    public class PaymentVerificationResult
    {
        public bool Success { get; set; }
        public string PaymentId { get; set; }
        public string Message { get; set; }
    }
}