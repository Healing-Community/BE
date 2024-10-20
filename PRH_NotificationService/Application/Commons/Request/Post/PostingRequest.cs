namespace Application.Commons.Request.Post
{
    public class PostingRequest
    {
        public required string PostId { get; set; }
        public required string UserId { get; set; }
        public string? Tittle { get; set; }
        public DateTime PostedDate { get; set; }
    }
}
