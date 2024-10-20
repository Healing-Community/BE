namespace Application.Commons.Request.Comment
{
    public class CommentRequest
    {
        public Guid CommentId { get; set; }
        public Guid PostId { get; set; }
        public Guid? ParentId { get; set; }
        public Guid UserId { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
