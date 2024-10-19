namespace Domain.Entities
{
    public class Reaction
    {
        public Guid ReactionId { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public Guid ReactionTypeId { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; init; }
        public DateTime? UpdateAt { get; set; }
        public Post Post { get; set; } 
        public ReactionType ReactionType { get; set; } 
    }
}
