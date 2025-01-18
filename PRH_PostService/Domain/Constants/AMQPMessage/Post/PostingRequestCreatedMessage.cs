namespace Domain.Constants.AMQPMessage.Post
{
    public class PostingRequestCreatedMessage
    {
        public required string PostingRequestId { get; set; }
        public required string UserId { get; set; }
        public string? UserName { get; set; }
        public string[]? FollowersId { get; set; }
        public string? Tittle { get; set; }
        public DateTime PostedDate { get; set; }
    }
}
