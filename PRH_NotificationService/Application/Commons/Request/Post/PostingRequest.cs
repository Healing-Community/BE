namespace Application.Commons.Request.Post
{
    public class PostingRequest
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string? Tittle { get; set; }
        public DateTime PostedDate { get; set; }
    }
}
