namespace Domain.Entities
{
    public class Notification
    {
        public Guid NotificationId { get; init; }
        public Guid UserId { get; init; }
        public Guid NotificationTypeId { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; set; }
        public bool IsRead { get; set; }
        public string Message { get; set; } = null!;

        public NotificationType NotificationType { get; set; } = null!;
    }
}
