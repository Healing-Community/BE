namespace Application.Commons.DTOs;

public class PaymentManagerDto
{
    public required string PaymentId { get; set; }
    public required string UserId { get; set; }
    public required string ExpertEmail { get; set; }
    public required string UserEmail { get; set; }
    public required string UserName { get; set; }
    public required string ExpertName { get; set; }
    public required string AppointmentId { get; set; }
    public required string AppointmentDate { get; set; }
    public required string StartTime { get; set; }
    public required string EndTime { get; set; }
    public long OrderCode { get; set; }
    public int Amount { get; set; }
    public int PaymemtStatus { get; set; }
    public int AppointmentStatus { get; set; }
    public string? UserPaymentQrCodeLink { get; set; }
    public string? ExpertPaymentQrCodeLink { get; set; }
    public string PaymentDetail { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public DateTime UpdatedAt { get; set; }
}
