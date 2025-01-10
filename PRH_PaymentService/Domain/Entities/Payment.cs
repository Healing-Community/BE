namespace Domain.Entities
{
    public class Payment
    {
        public required string PaymentId { get; set; }
        public required string UserId { get; set; }
        public required string AppointmentId { get; set; }
        public int ExpertAmount { get; set; }
        public long OrderCode { get; set; }
        public int Amount { get; set; }
        public int Status { get; set; }
        public string? UserPaymentQrCodeLink { get; set; }
        public string? ExpertPaymentQrCodeLink { get; set; }
        public string PaymentDetail { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
