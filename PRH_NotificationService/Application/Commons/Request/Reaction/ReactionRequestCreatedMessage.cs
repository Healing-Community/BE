namespace Application.Commons.Request.Reaction
{
    public class ReactionRequestCreatedMessage
    {
        public required string ReactionRequestId { get; set; }
        public required string UserId { get; set; }
        public required string PostId { get; set; }
        public required string ReactionTypeId { get; set; }
    }
}