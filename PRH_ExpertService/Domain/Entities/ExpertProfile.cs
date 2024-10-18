
namespace Domain.Entities
{
    public class ExpertProfile
    {
        public Guid ExpertProfileId { get; set; }
        public Guid UserId { get; set; }
        public int ExperienceYears { get; set; }
        public string? ExpertiseAreas { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
    }
}
