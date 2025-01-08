namespace Domain.Constants.AMQPMessage;
public class CommentReportMessage
{
    public required string UserId { get; set; }
    public required string CommentId { get; set; }
    public required string Content { get; set; }
    public required string PostId { get; set; }
    public required ReportTypeEnum ReportTypeEnum { get; set; }
}