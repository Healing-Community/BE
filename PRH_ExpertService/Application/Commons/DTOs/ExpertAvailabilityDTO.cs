namespace Application.Commons.DTOs
{
    public class ExpertAvailabilityDTO
    {
        public string ExpertAvailabilityId { get; set; }
        public string ExpertProfileId { get; set; }
        public DateOnly AvailableDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Amount { get; set; }
    }
}
