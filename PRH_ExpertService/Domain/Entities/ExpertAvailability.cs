namespace Domain.Entities
{
    public class ExpertAvailability
    {
        public required string ExpertAvailabilityId { get; set; }
        public required string ExpertProfileId { get; set; }
        public DateOnly AvailableDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Amount { get; set; }
        public ExpertProfile ExpertProfile { get; set; } = null!;
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}