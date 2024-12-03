using Application.Commons;
using MediatR;

namespace Application.Queries.Comments.CountTotalCommentByPostId
{
    public record CountTotalCommentByPostIdQuery(string PostId) : IRequest<BaseResponse<object>>;
}
