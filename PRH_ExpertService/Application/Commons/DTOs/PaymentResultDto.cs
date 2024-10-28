namespace Application.Commons.DTOs
{
    public class PaymentResultDto
    {
        public string ExpertAvailabilityId { get; set; } // ID của lịch trống
        public bool Success { get; set; } // Trạng thái thanh toán (thành công hoặc thất bại)
        public string PaymentId { get; set; } // ID giao dịch thanh toán
    }
}