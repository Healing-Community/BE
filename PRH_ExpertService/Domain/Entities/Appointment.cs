namespace Domain.Entities
{
    public class Appointment
    {
        public required string AppointmentId { get; set; }
        public required string UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public required string ExpertProfileId { get; set; }
        public string ExpertEmail { get; set; } = string.Empty;
        public required string ExpertAvailabilityId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int Status { get; set; }
        public string? MeetLink { get; set; }
        public string? RecordingLink { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ExpertProfile ExpertProfile { get; set; } = null!;
        public ExpertAvailability ExpertAvailability { get; set; } = null!;
    }
}