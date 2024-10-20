using Application.Commons;
using MediatR;

namespace Application.Commands.Comments.DeleteComment
{
    public record DeleteCommentCommand(Guid Id) : IRequest<BaseResponse<string>>;
}
