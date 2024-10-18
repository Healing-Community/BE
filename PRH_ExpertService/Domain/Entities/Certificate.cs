
namespace Domain.Entities
{
    public class Certificate
    {
        public Guid CertificateId { get; set; }
        public Guid ExpertProfileId { get; set; }
        public Guid CertificateTypeId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int Status { get; set; }

        public ExpertProfile ExpertProfile { get; set; } = null!;
        public CertificateType CertificateType { get; set; } = null!;
    }
}
