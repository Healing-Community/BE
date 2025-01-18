namespace Domain.Enum;

public enum AppointmentStatus
{
    PendingPayment = 0,
    Scheduled = 1,
    Canceled = 2,
    Completed = 3,
    CancelledPayment = 4,
    Refunded = 5,
    Reported = 6,
    ReportSuccess = 7,
    ReportFailed = 8,
    PayForUser = 9,
    PayForExpert = 10,
}
