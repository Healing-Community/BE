namespace Domain.Entities;

public class User
{
    public required string UserId { get; init; }
    public int RoleId { get; init; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string ProfilePicture { get; set; } = string.Empty;
    public string FullName { get; set; } =  string.Empty;
    public string PhoneNumber { get; set; } =  string.Empty;
    public string Descrtiption { get; set; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }
    public int Status { get; set; }
    // Navigation Property
    public Role Role { get; set; } = null!;
    public ICollection<Token> Tokens { get; set; } = [];
    public ICollection<SocialLink> SocialLinks { get; set; } = [];
    public ICollection<Follower> Followers { get; set; } = [];
    public PaymentInfo PaymentInfo { get; set; } = null!;
}