namespace Domain.Constants.AMQPMessage;

public class PostReportMessage
{
    public required string UserId { get; set; }
    public required string PostId { get; set; }    
    public required ReportTypeEnum ReportTypeEnum { get; set; }
}