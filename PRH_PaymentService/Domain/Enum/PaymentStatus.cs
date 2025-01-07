namespace Domain.Enum
{
    public enum PaymentStatus
    {
        Pending = 0, // Chờ thanh toán
        Paid = 1, // Đã tạo mã QR
        Completed = 2, // Đã thanh toán cho expert
        Cancelled = 3, // User hủy thanh toán và không tạo ra qr code
        Refunded = 4, // Hoàn lại tiền cho user
        Failed = 5,
        Unknown = 6
    }
}
