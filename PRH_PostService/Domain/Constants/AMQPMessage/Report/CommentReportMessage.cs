namespace Domain.Constants.AMQPMessage;
public class CommentReportMessage
{

    // User Report data
    public required string UserId { get; set; }
    public required string UserName { get; set; }
    public required string UserEmail { get; set; }

    // User has been reported data
    public required string ReportedUserId { get; set; }
    public required string ReportedUserName { get; set; }
    public required string ReportedUserEmail { get; set; }
    // Comment
    public required string CommentId { get; set; }
    public required string Content { get; set; }
    public required string PostId { get; set; }
    public required ReportTypeEnum ReportTypeEnum { get; set; }
}