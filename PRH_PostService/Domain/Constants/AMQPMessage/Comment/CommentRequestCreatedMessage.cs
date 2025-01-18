namespace Domain.Constants.AMQPMessage.Comment
{
    public class CommentRequestCreatedMessage
    {
        public required string CommentRequestId { get; set; }
        public required string CommentId { get; set; }
        public string? UserName { get; set; }
        public required string PostId { get; set; }
        public string? ParentId { get; set; }
        public required string UserId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime CommentedDate { get; set; }
    }
}
