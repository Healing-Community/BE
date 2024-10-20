using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Comments.GetComments
{
    public record GetCommentsQuery : IRequest<BaseResponse<IEnumerable<Comment>>>;
}
