namespace Application.Commons.DTOs
{
    public class UserGroupByGroupIdDto
    {
        public string UserId { get; set; }
        public string? GroupName { get; set; } 
        public string? GroupAvatar { get; set; } 
        public DateTime JoinedAt { get; set; }
        public string RoleInGroup { get; set; }
    }
}
