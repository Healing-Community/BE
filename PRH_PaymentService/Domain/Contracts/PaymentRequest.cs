namespace Domain.Contracts
{
    public class PaymentRequest
    {
        public required string AppointmentId { get; set; } // Xác định đơn hàng cần thanh toán
        public string? RedirectUrl { get; set; }
    }
}