namespace Application.Commons.Request.Report
{
    public class ReportRequest
    {
        public required string ReportId { get; set; }
        public string? PostId { get; set; }
        public string? UserId { get; set; }
        public string? ReportTypeId { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; set; }
    }
}
