using Application.Commons.DTOs;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commads_Queries.Queries.Reactions.GetCountReactions
{
    public class CountReactionsQueryHandler(IReactionRepository reactionRepository)
    : IRequestHandler<CountReactionsQuery, BaseResponse<ReactionCountDto>>
    {
        public async Task<BaseResponse<ReactionCountDto>> Handle(CountReactionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = request.UserId;

                // Đếm số reaction của user trên Post và Share
                var reactionsOnPosts = await reactionRepository.GetsByPropertyAsync(r => r.UserId == userId && r.PostId != null);
                var reactionsOnShares = await reactionRepository.GetsByPropertyAsync(r => r.UserId == userId && r.ShareId != null);

                var totalReactions = (reactionsOnPosts?.Count() ?? 0) + (reactionsOnShares?.Count() ?? 0);

                var result = new ReactionCountDto
                {
                    UserId = userId,
                    ReactionCount = totalReactions
                };

                return BaseResponse<ReactionCountDto>.SuccessReturn(result, "Tổng số lượt reaction đã được tính thành công.");
            }
            catch (Exception ex)
            {
                return BaseResponse<ReactionCountDto>.InternalServerError($"Lỗi khi đếm số lượt reaction: {ex.Message}");
            }
        }
    }


}
