namespace Domain.Entities
{
    public class Appointment
    {
        public required string AppointmentId { get; set; }
        public required string UserId { get; set; }
        public required string ExpertProfileId { get; set; }
        public required string ExpertAvailabilityId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Status { get; set; }
        public string? MeetLink { get; set; }
        public string? RecordingLink { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ExpertProfile ExpertProfile { get; set; } = null!;
        public ExpertAvailability ExpertAvailability { get; set; } = null!;
    }
}