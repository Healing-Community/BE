using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Comments.GetCommentsById
{
    public record GetCommentsByIdQuery(Guid Id) : IRequest<BaseResponse<Comment>>;
}
