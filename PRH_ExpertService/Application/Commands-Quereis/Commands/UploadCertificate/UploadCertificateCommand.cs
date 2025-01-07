using Application.Commons;
using MediatR;
using Microsoft.AspNetCore.Http;
using Application.Commons.DTOs;

namespace Application.Commands.UploadCertificate
{
    public record UploadCertificateCommand(IFormFile File, string CertificationTypeId) : IRequest<BaseResponse<UploadCertificateResponse>>;
}
