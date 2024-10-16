namespace Domain.Entities
{
    public class UserNotificationPreference
    {
        public Guid UserId { get; init; }
        public Guid NotificationTypeId { get; init; }
        public bool IsSubscribed { get; set; }

        public NotificationType NotificationType { get; set; } = null!;
    }
}
