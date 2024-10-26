namespace Domain.Entities
{
    public class ExpertProfile
    {
        public required string ExpertProfileId { get; set; }
        public required string UserId { get; set; }
        public string Specialization { get; set; } = string.Empty;
        public string ExpertiseAreas { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string FrontIdCardUrl { get; set; } = string.Empty;
        public string BackIdCardUrl { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public int Status { get; set; }
        public string RejectionReason { get; set; } = string.Empty;
        public decimal AverageRating { get; set; } = 0.0m;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<ExpertAvailability> ExpertAvailabilities { get; set; } = new List<ExpertAvailability>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<WorkExperience> WorkExperiences { get; set; } = new List<WorkExperience>();
        public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
    }
}