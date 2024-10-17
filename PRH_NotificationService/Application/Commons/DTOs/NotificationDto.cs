
namespace Application.Commons.DTOs
{
    public class NotificationDto
    {
        public Guid NotificationId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
