using System;

namespace Domain.Entities;

public class CommentReport
{
    public required string Id { get; set; }
    public required string UserId { get; set; }
    public required string CommentId { get; set; }
    public required string Content { get; set; }
    public required string PostId { get; set; }
    public required ReportTypeEnum reportTypeEnum { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
