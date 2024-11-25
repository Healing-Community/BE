namespace Domain.Entities
{
    public class Certificate
    {
        public required string CertificateId { get; set; }
        public required string ExpertProfileId { get; set; }
        public required string CertificateTypeId { get; set; }
        public string? VerifiedByAdminId { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public DateOnly? IssueDate { get; set; }
        public DateOnly? ExpirationDate { get; set; }
        public int Status { get; set; }
        public DateTime VerifiedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ExpertProfile ExpertProfile { get; set; } = null!;
        public CertificateType CertificateType { get; set; } = null!;
    }
}