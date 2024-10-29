namespace Domain.Entities
{
    public class UserGroup
    {
        public required string GroupId { get; set; }        
        public required string UserId { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.Now;
    }
}
