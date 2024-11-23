using Application.Commons;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.UploadCertificate
{
    public record UploadCertificateCommand(IFormFile File, string CertificationTypeId) : IRequest<DetailBaseResponse<string>>;
}
