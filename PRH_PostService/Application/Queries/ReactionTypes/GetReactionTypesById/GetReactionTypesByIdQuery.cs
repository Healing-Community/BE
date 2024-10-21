using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.ReactionTypes.GetReactionTypesById
{
    public record GetReactionTypesByIdQuery(string Id) : IRequest<BaseResponse<ReactionType>>;
}
