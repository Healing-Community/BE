using System;

namespace Domain.Constants.AMQPMessage.Report;

public class BanCommentMessage
{
    public required string CommentId { get; set; }
    public required string UserId { get; set; }
    public string? UserName { get; set; }
    public required string UserEmail { get; set; }
    public string? Content { get; set; }
    public bool IsApprove { get; set; }
}
