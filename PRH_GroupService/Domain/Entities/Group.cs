namespace Domain.Entities
{
    public class Group
    {
        public required string GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedByUserId { get; set; }
    }
}
