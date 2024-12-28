public class PaymentDetailsDTO
{
    // Thông tin Payment
    public required string PaymentId { get; set; }
    public required string AppointmentId { get; set; }
    public int Amount { get; set; }
    // ... field Payment khác nếu cần

    // Thông tin Appointment
    public string? ExpertName { get; set; }
    public string? ExpertEmail { get; set; }
    public string? AppointmentDate { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
}
