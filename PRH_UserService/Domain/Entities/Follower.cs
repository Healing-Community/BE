using System;

namespace Domain.Entities;

public class Follower
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string FollowerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public User User { get; set; } = null!;

    public Follower(string id, string userId, string followerId)
    {
        Id = id;
        UserId = userId;
        FollowerId = followerId;
        CreatedAt = DateTime.UtcNow + TimeSpan.FromHours(7);
        UpdatedAt = DateTime.UtcNow + TimeSpan.FromHours(7);
    }
}
