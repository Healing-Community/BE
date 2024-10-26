namespace Domain.Constants.AMQPMessage.Reaction
{
    public class ReactionRequest
    {
        public required string ReactionId { get; set; }
        public string? UserId { get; set; }
        public string? PostId { get; set; }
        public string? ReactionTypeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
