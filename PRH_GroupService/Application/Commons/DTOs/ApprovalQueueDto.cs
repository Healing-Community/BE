namespace Application.Commons.DTOs
{
    public class ApprovalQueueDto
    {
        public string QueueId { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
    }
}
