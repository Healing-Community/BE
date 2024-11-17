using Application.Commons;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.UploadCertificate
{
    public record UploadCertificateCommand(string ExpertId, IFormFile File, string CertificationTypeId) : IRequest<DetailBaseResponse<string>>;
}
