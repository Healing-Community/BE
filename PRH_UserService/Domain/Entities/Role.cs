namespace Domain.Entities
{
    public class Role
    {
        public int RoleId { get; init; }
        public string Name { get; set; } = null!;
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
