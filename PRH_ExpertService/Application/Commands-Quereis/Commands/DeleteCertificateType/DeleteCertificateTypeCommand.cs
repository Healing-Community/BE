using Application.Commons;
using MediatR;

namespace Application.Commands.DeleteCertificateType
{
    public record DeleteCertificateTypeCommand(string CertificateTypeId) : IRequest<BaseResponse<bool>>;
}