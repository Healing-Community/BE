using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.ReactionTypes.GetReactionTypesById
{
    public record GetReactionTypesByIdQuery(Guid Id) : IRequest<BaseResponse<ReactionType>>;
}
