using NUlid;
namespace Domain.Entities;
public class ChatMessages
{
    public string Id { get; set; } 
    public string SenderId { get; set; }
    public string RecipientId { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public string SessionId { get; set; }  // Để xác định session của cặp người dùng
    public ChatMessages(string senderId, string recipientId, string content, string sessionId)
    {
        Id = Ulid.NewUlid().ToString();
        SenderId = senderId;
        RecipientId = recipientId;
        Content = content;
        Timestamp = DateTime.UtcNow + TimeSpan.FromHours(7);
        SessionId = sessionId;
    }
}
