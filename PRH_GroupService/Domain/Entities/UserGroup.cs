namespace Domain.Entities
{
    public class UserGroup
    {
        public required string GroupId { get; set; }        
        public string? UserId { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.Now;
        public string RoleInGroup { get; set; } = "User";
    }
}
