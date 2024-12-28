using MediatR;
using Application.Commons;

namespace Application.Commands.DeleteCertificate
{
    public record DeleteCertificateCommand(string CertificateId) : IRequest<BaseResponse<bool>>;
}
