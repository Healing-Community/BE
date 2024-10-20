using Application.Commons;
using MediatR;

namespace Application.Commands.Posts.DeletePost
{
    public record DeletePostCommand(Guid Id) : IRequest<BaseResponse<string>>;
}
