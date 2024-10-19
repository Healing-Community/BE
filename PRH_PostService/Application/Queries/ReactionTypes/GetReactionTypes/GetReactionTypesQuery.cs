using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.ReactionTypes.GetReactionTypes
{
    public record GetReactionTypesQuery : IRequest<BaseResponse<IEnumerable<ReactionType>>>;
}
