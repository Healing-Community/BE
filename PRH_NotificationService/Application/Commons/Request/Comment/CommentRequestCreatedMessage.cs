namespace Application.Commons.Request.Comment
{
    public class CommentRequestCreatedMessage
    {
        public Guid CommentRequestId { get; set; }
        public Guid PostId { get; set; }
        //public Guid? ParentId { get; set; }
        public Guid UserId { get; set; }
        public string? Content { get; set; }
        public DateTime CommentedDate { get; set; }
    }
}