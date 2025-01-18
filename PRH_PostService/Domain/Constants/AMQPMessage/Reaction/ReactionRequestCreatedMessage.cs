namespace Domain.Constants.AMQPMessage.Reaction
{
    public class ReactionRequestCreatedMessage
    {
        public required string ReactionRequestId { get; set; }
        public string? UserReactionId { get; set; }
        public string? UserPostId { get; set; }
        public string? PostId { get; set; }
        public string? ReactionTypeId { get; set; }
        public string? UserName { get; set; }
        public string? Title { get; set; }
        public string? ReactionTypeName { get; set; } 
        public string? ReactionTypeIcon { get; set; }
        public DateTime ReactionDate { get; set; }
    }
}
