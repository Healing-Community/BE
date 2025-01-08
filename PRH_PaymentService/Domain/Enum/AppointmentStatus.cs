namespace Domain.Enum;

public enum AppointmentStatus
{
    PendingPayment = 0, // Chờ thanh toán
    Scheduled = 1, // Đã tạo mã QR
    Cancelled = 2, // Đã hủy
    Completed = 3, // Đã hoàn thành
    CancelPayment = 4, // Đã hủy thanh toán
    Refunded = 5, // Đã hoàn lại tiền
}
