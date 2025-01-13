using System;

namespace Domain.Entities;

public class PostReport
{
    public required string Id { get; set; }
    public required string UserId { get; set; }
    public required string UserName { get; set; }
    public required string UserEmail { get; set; }
    // reported user
    public required string ReportedUserId { get; set; }
    public required string ReportedUserName { get; set; }
    public required string ReportedUserEmail { get; set; }
    // Post
    public required string PostId { get; set; }  
    public required string PostTitle { get; set; }  
    public required ReportTypeEnum ReportTypeEnum { get; set; }

    public bool? IsApprove { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
