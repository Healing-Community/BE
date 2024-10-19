using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Reactions.GetReactions
{
    public record GetReactionsQuery : IRequest<BaseResponse<IEnumerable<Reaction>>>;
}
