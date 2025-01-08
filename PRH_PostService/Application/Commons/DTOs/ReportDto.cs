namespace Application.Commons.DTOs
{
    public class ReportDto
    {
        public required string PostId { get; set; }
        public required ReportTypeEnum ReportTypeEnum { get; set; }
    }
}
