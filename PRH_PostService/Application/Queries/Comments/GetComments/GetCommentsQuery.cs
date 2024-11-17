using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Queries.Comments.GetComments
{
    public record GetCommentsQuery : IRequest<BaseResponse<IEnumerable<CommentDtoResponse>>>;
}
