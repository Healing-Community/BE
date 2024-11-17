namespace Domain.Contracts
{
    public class PaymentRequest
    {
        public string AppointmentId { get; set; }
        public long OrderCode { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}