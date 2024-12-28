using System;

namespace Domain.Entities;

public class Share
{
    public required string ShareId { get; set; }
    public required string PostId { get; set; }
    public required string UserId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Post? Post { get; set; }
}
