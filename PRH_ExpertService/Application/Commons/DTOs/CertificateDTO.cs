namespace Application.Commons.DTOs
{
    public class CertificateDTO
    {
        public string CertificateId { get; set; }
        public string CertificateTypeId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int Status { get; set; }
    }
}
