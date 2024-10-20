
namespace Application.Commons.DTOs
{
    public class NotificationDto
    {
        public required string NotificationId { get; set; }
        public required string NotificationTypeId { get; set; }
        public required string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
