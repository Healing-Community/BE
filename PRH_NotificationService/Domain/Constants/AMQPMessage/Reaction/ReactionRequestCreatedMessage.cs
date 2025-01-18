namespace Domain.Constants.AMQPMessage.Reaction
{
    public class ReactionRequestCreatedMessage
    {
        public required string ReactionRequestId { get; set; }
        public string? UserId { get; set; }
        public string? PostId { get; set; }
        public string? ReactionTypeId { get; set; }
        public string? UserName { get; set; }
        public string? Title { get; set; }
        public string? ReactionTypeName { get; set; }
        public string? ReactionTypeIcon { get; set; }
        public DateTime ReactionDate { get; set; }
    }
}
