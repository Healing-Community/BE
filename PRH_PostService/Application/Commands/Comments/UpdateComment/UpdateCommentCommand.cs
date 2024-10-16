using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.Comments.UpdateComment
{
    public record UpdateCommentCommand(Guid commentId, CommentDto CommentDto) : IRequest<BaseResponse<string>>;
}
