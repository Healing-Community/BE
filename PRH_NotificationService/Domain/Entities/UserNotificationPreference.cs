namespace Domain.Entities
{
    public class UserNotificationPreference
    {
        public required string UserId { get; init; }
        public required string NotificationTypeId { get; init; }
        public bool IsSubscribed { get; set; }

        public NotificationType NotificationType { get; set; } = null!;
    }
}
