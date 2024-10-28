namespace Application.Commons.Payment
{
    public class PaymentRequestResult
    {
        public bool Success { get; set; }
        public string PaymentId { get; set; }
        public string QrCodeLink { get; set; }
        public string Message { get; set; }
    }
}