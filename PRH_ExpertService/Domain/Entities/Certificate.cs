
namespace Domain.Entities
{
    public class Certificate
    {
        public required string CertificateId { get; set; }
        public required string ExpertProfileId { get; set; }
        public required string CertificateTypeId { get; set; }
        public string? FileUrl { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int Status { get; set; }

        public ExpertProfile ExpertProfile { get; set; } = null!;
        public CertificateType CertificateType { get; set; } = null!;
    }
}
