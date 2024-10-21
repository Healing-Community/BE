using Application.Commons;
using MediatR;

namespace Application.Commands.Comments.DeleteComment
{
    public record DeleteCommentCommand(string Id) : IRequest<BaseResponse<string>>;
}
