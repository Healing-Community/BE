namespace Domain.Constants.AMQPMessage;

public class ReportMessage
{
    public required string UserId { get; set; }
    public string? TargetUserId { get; set; }
    public string? ExpertId { get; set; }
    public string? PostId { get; set; }
    public string? CommentId { get; set; }
    public string? Description { get; set; }
    
    public required string ReportTypeId { get; set; }
}