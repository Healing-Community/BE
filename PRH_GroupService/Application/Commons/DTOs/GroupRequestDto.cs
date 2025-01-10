namespace Application.Commons.DTOs
{
    public class GroupRequestDto
    {
        public string? GroupRequestId { get; set; }
        public string? GroupName { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? CoverImg { get; set; }
        public bool? IsApproved { get; set; } // Pending = null, Approved = true, Rejected = false
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedById { get; set; }
        public string? RequestedById { get; set; }
        public DateTime? RequestedAt { get; set; }
    }
}
