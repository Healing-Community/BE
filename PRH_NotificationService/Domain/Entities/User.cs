namespace Domain.Entities
{
    public class User
    {
        public Guid UserId { get; init; }
        public int RoleId { get; init; }
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public int Status { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; set; }

        public Role Role { get; set; } = null!;
        public ICollection<Notification> Notifications { get; set; } = null!;
        public ICollection<UserNotificationPreference> NotificationPreferences { get; set; } = null!;
    }
}
