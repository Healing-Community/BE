using System;

namespace Domain.Enum;

public enum AppointmentStatusEnum
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
}
