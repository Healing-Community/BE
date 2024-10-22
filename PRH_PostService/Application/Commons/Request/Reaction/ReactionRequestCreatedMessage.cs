namespace Application.Commons.Request.Reaction
{
    public class ReactionRequestCreatedMessage
    {
        public required string ReactionRequestId { get; set; }
        public string? UserId { get; set; }
        public string? PostId { get; set; }
        public string? ReactionTypeId { get; set; }
        public DateTime ReactionDate { get; set; }
    }
}
