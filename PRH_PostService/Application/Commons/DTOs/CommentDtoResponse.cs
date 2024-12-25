namespace Application.Commons.DTOs
{
    public class CommentDtoResponse
    {
        public string CommentId { get; set; }
        public string? PostId { get; set; }
        public string? ShareId { get; set; }
        public string? ParentId { get; set; }
        public string? UserId { get; set; }
        public string Content { get; set; }
        public string? CoverImgUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<CommentDtoResponse> Replies { get; set; } = new();
    }
}
