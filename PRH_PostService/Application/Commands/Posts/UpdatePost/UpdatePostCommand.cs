using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.Posts.UpdatePost
{
    public record UpdatePostCommand(Guid postId, PostDto PostDto) : IRequest<BaseResponse<string>>;
}
