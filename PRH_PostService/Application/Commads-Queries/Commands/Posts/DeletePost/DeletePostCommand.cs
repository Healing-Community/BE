using Application.Commons;
using MediatR;

namespace Application.Commands.Posts.DeletePost
{
    public record DeletePostCommand(string Id) : IRequest<BaseResponse<string>>;
}
