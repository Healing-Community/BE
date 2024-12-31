using NUlid;

namespace Domain.Entities
{
    public class GroupCreationRequest
    {
        public string GroupRequestId { get; set; } = Ulid.NewUlid().ToString();
        public string RequestedById { get; set; } = string.Empty; // User ID who made the request
        public string GroupName { get; set; } = string.Empty; 
        public string Description { get; set; } = string.Empty; 
        public DateTime? RequestedAt { get; set; } 
        public bool? IsApproved { get; set; } = null; // NULL: pending, TRUE: approved, FALSE: rejected
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedById { get; set; } // Moderator/Admin ID
    }
}
