namespace Domain.Entities
{
    public class WorkExperience
    {
        public required string WorkExperienceId { get; set; }
        public required string ExpertProfileId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string PositionTitle { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ExpertProfile ExpertProfile { get; set; } = null!;
    }
}