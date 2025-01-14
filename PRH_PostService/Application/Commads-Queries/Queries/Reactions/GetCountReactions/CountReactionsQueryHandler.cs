using Application.Commons.DTOs;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using Domain.Entities;

namespace Application.Commads_Queries.Queries.Reactions.GetCountReactions
{
    public class CountReactionsQueryHandler(IPostRepository postRepository, IReactionRepository reactionRepository, IShareRepository shareRepository)
        : IRequestHandler<CountReactionsQuery, BaseResponse<ReactionCountDto>>
    {
        public async Task<BaseResponse<ReactionCountDto>> Handle(CountReactionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = request.UserId;

                // Lấy tất cả bài post của user
                var userPosts = await postRepository.GetsByPropertyAsync(p => p.UserId == userId);

                // Lấy tất cả bài share của user (sử dụng tên đầy đủ của class Share để tránh xung đột)
                var userShares = await shareRepository.GetsByPropertyAsync(s => s.UserId == userId) ?? new List<Domain.Entities.Share>();

                // Đếm lượt react vào các bài post của user
                int reactionsOnPosts = 0;
                foreach (var post in userPosts ?? new List<Post>())
                {
                    var reactions = await reactionRepository.GetsByPropertyAsync(r => r.PostId == post.PostId && r.UserId != userId);
                    reactionsOnPosts += reactions?.Count() ?? 0;
                }

                // Đếm lượt react vào các bài share của user (sử dụng tên đầy đủ của class Share)
                int reactionsOnShares = 0;
                foreach (var share in userShares)
                {
                    var reactions = await reactionRepository.GetsByPropertyAsync(r => r.ShareId == share.ShareId && r.UserId != userId);
                    reactionsOnShares += reactions?.Count() ?? 0;
                }

                // Tổng số lượt react
                var totalReactions = reactionsOnPosts + reactionsOnShares;

                // Kết quả trả về
                var result = new ReactionCountDto
                {
                    UserId = userId,
                    ReactionCount = totalReactions
                };

                return BaseResponse<ReactionCountDto>.SuccessReturn(result, "Tổng số lượt reaction nhận được đã được tính thành công.");
            }
            catch (Exception ex)
            {
                return BaseResponse<ReactionCountDto>.InternalServerError($"Lỗi khi đếm số lượt reaction: {ex.Message}");
            }
        }
    }


}
