using Application.Commons.DTOs;
using Application.Commons;
using MediatR;

namespace Application.Commads_Queries.Queries.Comments.GetCommentsByShareId
{
    public record GetCommentsByShareIdQuery(string ShareId) : IRequest<BaseResponse<List<CommentDtoResponse>>>;
}
