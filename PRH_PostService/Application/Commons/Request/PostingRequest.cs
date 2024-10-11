namespace Application.Commons.Request
{
    public class PostingRequest
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string? Tittle { get; set; }
        public DateTime PostedDate { get; set; }

    }
}
