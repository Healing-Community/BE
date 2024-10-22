namespace Application.Commons.Request.Report
{
    public class ReportRequestCreatedMessage
    {
        public required string ReportRequestId { get; set; }
        public string? PostId { get; set; }
        public string? UserId { get; set; }
        public string? ReportTypeId { get; set; }
        public int Status { get; set; }
        public DateTime ReportedDate { get; set; }
    }
}
