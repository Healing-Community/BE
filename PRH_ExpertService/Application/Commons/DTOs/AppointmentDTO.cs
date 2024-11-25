namespace Application.Commons.DTOs
{
    public class AppointmentDTO
    {
        public string AppointmentId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string UserEmail { get; set; }
        public string MeetLink { get; set; }
        public int Status { get; set; }
    }
}