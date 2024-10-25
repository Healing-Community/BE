using Application.Commons;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.UploadFile
{
    public record UploadFileCommand(string ExpertId, IFormFile File, string CertificationTypeId) : IRequest<BaseResponse<string>>;
}
