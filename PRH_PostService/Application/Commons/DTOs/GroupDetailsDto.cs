namespace Application.Commons.DTOs
{
    public class GroupDetailsDto
    {
        public string GroupId { get; set; } = string.Empty;
        public int Visibility { get; set; } // 0: Public, 1: Private
    }
}
