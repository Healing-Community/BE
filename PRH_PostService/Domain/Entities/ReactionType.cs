namespace Domain.Entities
{
    public class ReactionType
    {
        public required string ReactionTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public ICollection<Reaction>? Reactions { get; set; } 
    }
}
