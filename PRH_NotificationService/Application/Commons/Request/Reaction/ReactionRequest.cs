namespace Application.Commons.Request.Reaction
{
    public class ReactionRequest
    {
        public required string ReactionId { get; set; }
        public required string UserId { get; set; }
        public required string PostId { get; set; }
        public required string ReactionTypeId { get; set; }

    }
}