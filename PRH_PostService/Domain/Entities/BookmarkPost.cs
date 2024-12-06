using System;

namespace Domain.Entities;

public class BookmarkPost
{
    public required string BookmarkPostId { get; set; }
    public required string PostId { get; set; }
    public required string BookmarkId { get; set; }

    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }

    public Bookmark? Bookmark { get; set; }
    public Post? Post { get; set; }
}
