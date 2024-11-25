namespace Application.Commons.DTOs
{
    public class CertificateDTO
    {
        public string CertificateId { get; set; }
        public string CertificateTypeId { get; set; }
        public DateOnly IssueDate { get; set; }
        public DateOnly ExpirationDate { get; set; }
        public int Status { get; set; }
        public string FileUrl { get; set; }
    }
}