namespace Application.Commons.Request.Post
{
    public class PostingRequestCreatedMessage
    {
        public required string PostingRequestId { get; set; }
        public required string UserId { get; set; }
        public string? Tittle { get; set; }
        public DateTime PostedDate { get; set; }
    }
}