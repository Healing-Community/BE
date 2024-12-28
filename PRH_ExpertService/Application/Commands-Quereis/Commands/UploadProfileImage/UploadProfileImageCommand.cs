using MediatR;
using Microsoft.AspNetCore.Http;
using Application.Commons;

namespace Application.Commands.UploadProfileImage
{
    public record UploadProfileImageCommand(IFormFile File) : IRequest<DetailBaseResponse<string>>;
}
