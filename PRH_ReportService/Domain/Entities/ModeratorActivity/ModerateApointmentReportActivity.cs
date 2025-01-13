using System;

namespace Domain.Entities.ModeratorActivity;

public class ModerateApointmentReportActivity
{
    public required string Id { get; set; }
    public required string ModeratorId { get; set; }
    public required string ModeratorName { get; set; }
    public required string ModeratorEmail { get; set; }
    public required string AppointmentId { get; set; }
    public DateOnly AppoinmtentDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string? UserEmail { get; set; }
    public string? UserName { get; set; }
    public string? ExpertEmail { get; set; }
    public string? ExpertName { get; set; }

    public bool IsApprove { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
