namespace Application.Commons.DTOs
{
    public class CreateGroupRequestDto
    {
        public required string GroupName { get; set; }
        public string Description { get; set; } = string.Empty;
        public string CoverImg { get; set; } = string.Empty;
    }
}
