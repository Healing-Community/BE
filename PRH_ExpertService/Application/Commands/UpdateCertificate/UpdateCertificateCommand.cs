using Application.Commons;
using MediatR;

namespace Application.Commands.UpdateCertificate
{
    public record UpdateCertificateCommand(string CertificateId, DateTime? IssueDate, DateTime? ExpirationDate, string CertificateTypeId) : IRequest<DetailBaseResponse<bool>>;
}