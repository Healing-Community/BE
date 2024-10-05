namespace Domain.Entities
{
    public class Category
    {
        public Guid Id { get; init; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreateAt { get; init; }
        public DateTime? UpdateAt { get; set; }

        public ICollection<Post> posts { get; set; } = [];
    }
}
