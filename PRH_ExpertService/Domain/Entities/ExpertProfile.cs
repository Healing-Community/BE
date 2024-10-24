﻿
namespace Domain.Entities
{
    public class ExpertProfile
    {
        public required string ExpertProfileId { get; set; }
        public required string UserId { get; set; }
        public int ExperienceYears { get; set; }
        public string? ExpertiseAreas { get; set; }
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
    }
}
