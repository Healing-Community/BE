namespace Domain.Entities
{
    public class Group
    {
        public required string GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public DateTime CreateAt { get; init; }
        public DateTime? UpdateAt { get; set; }
    }
}
