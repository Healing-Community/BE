using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Reactions.GetReactionsById
{
    public record GetReactionsByIdQuery(string Id) : IRequest<BaseResponse<Reaction>>;
}
