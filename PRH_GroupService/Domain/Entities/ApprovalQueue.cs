namespace Domain.Entities
{
    public class ApprovalQueue
    {
        public required string QueueId { get; set; }
        public required string GroupId { get; set; } 
        public required string UserId { get; set; }
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public bool IsApproved { get; set; } = false;
    }
}
