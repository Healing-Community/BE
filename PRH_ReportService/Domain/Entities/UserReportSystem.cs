using System;

namespace Domain.Entities;

public class UserReportSystem
{
    public required string Id { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; } 
    public DateTime UpdatedAt { get; set; } 
}
