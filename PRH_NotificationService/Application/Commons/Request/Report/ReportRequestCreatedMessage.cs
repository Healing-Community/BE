namespace Application.Commons.Request.Report
{
    public class ReportRequestCreatedMessage
    {
        public Guid ReportRequestId { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public Guid ReportTypeId { get; set; }
        public int Status { get; set; }
        public DateTime ReportedDate { get; set; }
    }
}