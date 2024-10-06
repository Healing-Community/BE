namespace Domain.Entities
{
    public class User
    {
        public Guid UserId { get; init; }
        public int RoleId { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? FullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Status { get; set; }
        public Role Role { get; set; } = null!;
        public ICollection<Token> Tokens { get; set; } = new List<Token>();
    }
}
