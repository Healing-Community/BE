namespace Application.Commons.Request.Comment
{
    public class CommentRequest
    {
        public required string CommentId { get; set; }
        public string? PostId { get; set; }
        public string? ParentId { get; set; }
        public string? UserId { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
