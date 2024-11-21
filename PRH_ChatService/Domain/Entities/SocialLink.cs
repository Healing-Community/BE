namespace Domain.Entities;

public class SocialLink
{
    public required string LinkId { get; set; }
    public required string UserId { get; set; }
    public required string PlatformName { get; set; }
    public required string Url { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }
    public User User { get; set; } = null!;
}
