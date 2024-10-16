namespace Domain.Entities
{
    public class Role
    {
        public int RoleId { get; init; }
        public string RoleName { get; set; } = null!;

        public ICollection<User> Users { get; set; } = null!;
    }
}
