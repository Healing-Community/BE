using MediatR;
using Application.Commons;

namespace Application.Commands.RejectCertificate
{
    public class RejectCertificateCommand : IRequest<BaseResponse<bool>>
    {
        public required string CertificateId { get; set; }
    }
}
