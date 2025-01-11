namespace Domain.Constants.AMQPMessage.Report;

public class SyncBanPostReportMessage
{
    public required string PostId { get; set; }
    public bool IsApprove { get; set; }
}
