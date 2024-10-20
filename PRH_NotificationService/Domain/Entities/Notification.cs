namespace Domain.Entities
{
    public class Notification
    {
        public required string NotificationId { get; init; }
        public required string UserId { get; init; }
        public required string NotificationTypeId { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; set; }
        public bool IsRead { get; set; }
        public string Message { get; set; } = null!;

        public NotificationType NotificationType { get; set; } = null!;
    }
}
