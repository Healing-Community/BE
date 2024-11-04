namespace Domain.Entities
{
    public class Payment
    {
        public required string PaymentId { get; set; }
        public required string UserId { get; set; }
        public required string AppointmentId { get; set; }
        public decimal Amount { get; set; }
        public int Status { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
