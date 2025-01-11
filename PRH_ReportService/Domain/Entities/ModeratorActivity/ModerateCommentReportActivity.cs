using System;

namespace Domain.Entities.ModeratorActivity;

public class ModerateCommentReportActivity
{
    public required string Id { get; set; }
    public required string CommentId { get; set; }
    public required string UserId { get; set; }
    public string? UserName { get; set; }
    public required string UserEmail { get; set; }
    public string? Content { get; set; }
    public bool IsApprove { get; set; }

     public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
