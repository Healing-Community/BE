using System;

namespace Domain.Constants.AMQPMessage.Report;

public class BanPostMessage
{
    public required string PostId { get; set; }
    public string? PostTitle { get; set; }
    public required string UserId { get; set; }  // Moderator Id
    public string? UserName { get; set; } // Moderator Name
    public required string UserEmail { get; set; } // Moderator Email
    public string? Reason { get; set; }
    public bool IsApprove { get; set; }
}
