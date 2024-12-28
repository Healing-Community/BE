namespace Application.Commons.DTOs
{
    public class RoleCountDto
    {
        public int TotalUsers { get; set; }
        public int TotalOwnersAndModerators { get; set; }
        public int TotalMembers => TotalUsers + TotalOwnersAndModerators;
    }
}
