using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.Posts.UpdatePost
{
    public record UpdatePostCommand(string PostId, PostDto PostDto) : IRequest<BaseResponse<string>>;
}
