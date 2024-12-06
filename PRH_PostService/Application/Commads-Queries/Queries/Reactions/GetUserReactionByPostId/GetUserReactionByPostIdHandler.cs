using System;
using System.Security.Claims;
using Application.Commons;
using Application.Commons.Enum;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commads_Queries.Queries.Reactions.GetUserReactionByPostId;

public class GetUserReactionByPostIdHandlerQuery(IReactionRepository reactionRepository, IHttpContextAccessor accessor) : IRequestHandler<GetUserReactionByPostIdQuery, BaseResponse<Reaction>>
{
    public async Task<BaseResponse<Reaction>> Handle(GetUserReactionByPostIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reaction = await reactionRepository.GetByPropertyAsync(x => x.PostId == request.PostId && x.UserId == userId);
            reaction.ReactionType = new ReactionType{
                ReactionTypeId = reaction?.ReactionTypeId ?? "1",
                Name = Enum.GetName(typeof(ReactionTypeEnum), int.Parse(reaction?.ReactionTypeId))
            };
            if (reaction == null)
            {
                return BaseResponse<Reaction>.SuccessReturn();
            }
            return BaseResponse<Reaction>.SuccessReturn(reaction);
        }
        catch (Exception ex)
        {
            return BaseResponse<Reaction>.SuccessReturn();
        }
    }
}