namespace Domain.Constants.AMQPMessage.Report;

public class SyncModerateAppointmentMessage
{
    public required string AppointmentId { get; set; }
    public bool IsApprove { get; set; }
}
