using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Commads_Queries.Queries.Reactions.GetUserReactionByShareId
{
    public record GetUserReactionByShareIdQuery(string ShareId) : IRequest<BaseResponse<Reaction>>;
}
