namespace Domain.Entities
{
    public class Token
    {
        public Guid TokenId { get; init; }
        public Guid UserId { get; set; }
        public string RefreshTokenHash { get; set; } = null!;
        public DateTime IssuedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int Status { get; set; }
        public bool IsUsed { get; set; }
        public User User { get; set; } = null!;
    }
}
