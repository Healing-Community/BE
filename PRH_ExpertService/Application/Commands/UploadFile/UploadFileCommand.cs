using Application.Commons;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.UploadFile
{
    public record UploadFileCommand(string ExpertId, IFormFile File) : IRequest<BaseResponse<string>>;
}
