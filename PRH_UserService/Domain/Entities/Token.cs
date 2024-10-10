namespace Domain.Entities
{
    public class Token
    {
        public Guid UserId { get; init; }
        public string RefreshToken { get; set; } = null!;
        public DateTime IssuedAt { get; init; }
        public DateTime ExpiresAt { get; set; }
        public int Status { get; set; }
        public bool IsUsed { get; set; }
        public User User { get; set; } = null!;
    }
}
