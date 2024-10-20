namespace Domain.Entities
{
    public class ReactionType
    {
        public Guid ReactionTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Reaction> Reactions { get; set; } 
    }

}
