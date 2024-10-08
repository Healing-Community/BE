namespace Domain.Entities
{
    public class Token
    {
        public Guid TokenId { get; init; }
        public Guid UserId { get; init; }
        public string RefreshTokenHash { get; set; } = null!;
        public DateTime IssuedAt { get; init; }
        public DateTime ExpiresAt { get; set; }
        public int Status { get; set; }
        public bool IsUsed { get; set; }
        public User User { get; set; } = null!;
    }
}
