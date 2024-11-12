using Application.Commons;
using MediatR;

namespace Application.Commands.UpdateCertificateType
{
    public record UpdateCertificateTypeCommand(string CertificateTypeId, string Name, string Description, bool IsMandatory) : IRequest<DetailBaseResponse<bool>>;
}