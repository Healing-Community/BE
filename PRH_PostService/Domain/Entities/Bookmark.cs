namespace Domain.Entities;

public class Bookmark
{
    public required string BookmarkId { get; set; }
    public required string UserId { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<BookmarkPost>? BookmarkPosts { get; set; }
}
