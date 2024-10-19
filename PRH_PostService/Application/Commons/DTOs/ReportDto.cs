namespace Application.Commons.DTOs
{
    public class ReportDto
    {
        public Guid PostId { get; set; }
        public Guid ReportTypeId { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; set; }
    }
}
