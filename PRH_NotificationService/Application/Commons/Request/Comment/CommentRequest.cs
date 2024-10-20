namespace Application.Commons.Request.Comment
{
    public class CommentRequest
    {
        public required string CommentId { get; set; }
        public required string PostId { get; set; }
        public required string ParentId { get; set; }
        public required string UserId { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}