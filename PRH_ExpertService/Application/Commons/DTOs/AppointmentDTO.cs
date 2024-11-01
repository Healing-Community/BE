namespace Application.Commons.DTOs
{
    public class AppointmentDTO
    {
        public string AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}