namespace Domain.Constants.AMQPMessage;

public class ReportMessage
{
    public Guid UserId { get; set; }
    public Guid TargetUserId { get; set; }
    public Guid ExpertId { get; set; }
    public Guid PostId { get; set; }
    public Guid CommentId { get; set; }
    public string? Description { get; set; }
    
    public required string ReportTypeId { get; set; }
}