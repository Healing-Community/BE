using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.Posts.UpdatePost
{
    public record UpdatePostCommand(string postId, PostDto PostDto) : IRequest<BaseResponse<string>>;
}
