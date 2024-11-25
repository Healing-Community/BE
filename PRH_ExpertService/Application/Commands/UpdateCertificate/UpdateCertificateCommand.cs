using Application.Commons;
using MediatR;

namespace Application.Commands.UpdateCertificate
{
    public record UpdateCertificateCommand(string CertificateId, DateOnly? IssueDate, DateOnly? ExpirationDate, string CertificateTypeId) : IRequest<DetailBaseResponse<bool>>;
}