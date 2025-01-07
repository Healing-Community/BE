using MediatR;
using Application.Commons;

namespace Application.Commands.ApproveCertificate
{
    public class ApproveCertificateCommand : IRequest<BaseResponse<bool>>
    {
        public required string CertificateId { get; set; }
    }
}
