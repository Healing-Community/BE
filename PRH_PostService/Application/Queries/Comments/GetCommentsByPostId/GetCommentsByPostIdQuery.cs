using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Queries.Comments.GetCommentsByPostId
{
    public record GetCommentsByPostIdQuery(string PostId) : IRequest<BaseResponse<List<CommentDtoResponse>>>;
}
