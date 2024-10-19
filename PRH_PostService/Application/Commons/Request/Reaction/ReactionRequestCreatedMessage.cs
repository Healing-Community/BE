namespace Application.Commons.Request.Reaction
{
    public class ReactionRequestCreatedMessage
    {
        public Guid ReactionRequestId { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public Guid ReactionTypeId { get; set; }
    }
}
