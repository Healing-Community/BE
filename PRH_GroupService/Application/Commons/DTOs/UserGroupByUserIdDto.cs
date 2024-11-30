namespace Application.Commons.DTOs
{
    public class UserGroupByUserIdDto
    {
        public string GroupId { get; set; }
        public DateTime JoinedAt { get; set; }
        public string RoleInGroup { get; set; }
    }
}
