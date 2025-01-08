namespace Application.Commons.DTOs
{
    public class GroupWithPostCountDto
    {
        public string GroupId { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public int PostCount { get; set; }
    }
}
