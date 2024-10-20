
namespace Domain.Entities
{
    public class CertificateType
    {
        public required string CertificateTypeId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
    }
}
