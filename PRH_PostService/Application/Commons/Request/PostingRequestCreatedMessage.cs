namespace Application.Commons.Request
{
    public class PostingRequestCreatedMessage
    {
        public Guid PostingRequestId { get; set; }
        public Guid UserId { get; set; }
        public string? Tittle { get; set; }
        public DateTime PostedDate { get; set; }
    }
}
