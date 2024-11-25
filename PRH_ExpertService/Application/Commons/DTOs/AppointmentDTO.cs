namespace Application.Commons.DTOs
{
    public class AppointmentDTO
    {
        public string AppointmentId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}