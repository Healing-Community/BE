using MediatR;
using Application.Commons;

namespace Application.Commands.ApproveCertificate
{
    public class ApproveCertificateCommand : IRequest<DetailBaseResponse<bool>>
    {
        public required string CertificateId { get; set; }
    }
}
