namespace Application.Commons.DTOs
{
    public class ActivityReportDTO
    {
        public DateOnly Date { get; set; }
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public double CompletionRate { get; set; }
        public string Status { get; set; }
    }
}
