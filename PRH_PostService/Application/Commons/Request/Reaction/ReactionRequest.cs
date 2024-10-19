namespace Application.Commons.Request.Reaction
{
    public class ReactionRequest
    {
        public Guid ReactionId { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public Guid ReactionTypeId { get; set; }

    }
}
