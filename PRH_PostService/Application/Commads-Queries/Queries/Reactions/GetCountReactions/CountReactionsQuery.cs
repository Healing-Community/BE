using Application.Commons.DTOs;
using Application.Commons;
using MediatR;

namespace Application.Commads_Queries.Queries.Reactions.GetCountReactions
{
    public record CountReactionsQuery(string UserId) : IRequest<BaseResponse<ReactionCountDto>>;
}
