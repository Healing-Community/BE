namespace Domain.Entities
{
    public class NotificationType
    {
        public required string NotificationTypeId { get; init; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public ICollection<Notification> Notifications { get; set; } = null!;
        public ICollection<UserNotificationPreference> UserPreferences { get; set; } = null!;
    }
}
