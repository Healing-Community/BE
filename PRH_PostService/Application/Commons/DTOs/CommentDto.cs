namespace Application.Commons.DTOs
{
    public class CommentDto
    {
        public required string PostId { get; set; }
        public  string? ParentId { get; set; }
        public string Content { get; set; } = null!;
        public string? CoverImgUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
