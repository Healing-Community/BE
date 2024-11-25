namespace Domain.Entities
{
    public class Category
    {
        public required string CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreateAt { get; init; }
        public DateTime? UpdateAt { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<UserPreference> UserPreferences { get; set; }
    }
}
