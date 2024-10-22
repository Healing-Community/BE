namespace Application.Commons.DTOs
{
    public class ReportDto
    {
        public string? PostId { get; set; }
        public string? ReportTypeId { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; set; }
    }
}
