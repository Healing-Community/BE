using Application.Commons;
using MediatR;

namespace Application.Commads_Queries.Queries.Comments.CountTotalCommentByShareId
{
    public record CountTotalCommentByShareIdQuery(string ShareId) : IRequest<BaseResponse<object>>;
}
