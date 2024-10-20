namespace Application.Commons.Request.Report
{
    public class ReportRequest
    {
        public required string ReportId { get; set; }
        public required string PostId { get; set; }
        public required string UserId { get; set; }
        public required string ReportTypeId { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; set; }
    }
}