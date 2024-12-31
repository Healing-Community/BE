using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace Application.Commads_Queries.Queries.Reactions.GetUserReactionByShareId
{
    public class GetUserReactionByShareIdQueryHandler(
        IReactionTypeRepository reactionTypeRepository,
        IReactionRepository reactionRepository,
        IHttpContextAccessor accessor)
        : IRequestHandler<GetUserReactionByShareIdQuery, BaseResponse<Reaction>>
    {
        public async Task<BaseResponse<Reaction>> Handle(GetUserReactionByShareIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var reaction = await reactionRepository.GetByPropertyAsync(x => x.ShareId == request.ShareId && x.UserId == userId);
                var reactionTypeIndb = await reactionTypeRepository.GetByPropertyAsync(r => r.ReactionTypeId == reaction.ReactionTypeId);
                if (reactionTypeIndb == null || reaction == null)
                {
                    return BaseResponse<Reaction>.NotFound("Reaction không tồn tại.");
                }
                reaction.ReactionType = new ReactionType
                {
                    ReactionTypeId = reaction?.ReactionTypeId ?? "1",
                    Name = reactionTypeIndb.Name,
                    Icon = reactionTypeIndb.Icon

                };
                if (reaction == null)
                {
                    return BaseResponse<Reaction>.SuccessReturn();
                }
                return BaseResponse<Reaction>.SuccessReturn(reaction);
            }
            catch
            {
                return BaseResponse<Reaction>.SuccessReturn();
            }
        }
    }
}
