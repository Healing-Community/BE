namespace Application.Commons.Request.Comment
{
    public class CommentRequestCreatedMessage
    {
        public required string CommentRequestId { get; set; }
        public required string PostId { get; set; }
        //public Guid? ParentId { get; set; }
        public required string UserId { get; set; }
        public string? Content { get; set; }
        public DateTime CommentedDate { get; set; }
    }
}
