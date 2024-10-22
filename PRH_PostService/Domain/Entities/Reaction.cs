namespace Domain.Entities
{
    public class Reaction
    {
        public required string ReactionId { get; set; }
        public string? UserId { get; set; }
        public string? PostId { get; set; }
        public string? ReactionTypeId { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; init; }
        public DateTime? UpdateAt { get; set; }
        public Post Post { get; set; } 
        public ReactionType ReactionType { get; set; } 
    }
}
