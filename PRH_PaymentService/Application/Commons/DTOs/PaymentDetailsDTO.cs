public class PaymentDetailsDTO
{
    // Thông tin Payment
    public string PaymentId { get; set; }
    public string AppointmentId { get; set; }
    public decimal Amount { get; set; }
    // ... field Payment khác nếu cần

    // Thông tin Appointment
    public string ExpertName { get; set; }
    public string ExpertEmail { get; set; }
    public string AppointmentDate { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
}
