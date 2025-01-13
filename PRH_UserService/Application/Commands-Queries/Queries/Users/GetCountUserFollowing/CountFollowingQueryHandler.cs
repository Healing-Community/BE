using Application.Commons.DTOs;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commands_Queries.Queries.Users.GetCountUserFollowing
{
    public class CountFollowingQueryHandler(IFollowerRepository followerRepository)
        : IRequestHandler<CountFollowingQuery, BaseResponse<FollowingCountDto>>
    {
        public async Task<BaseResponse<FollowingCountDto>> Handle(CountFollowingQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = request.UserId;
                var followings = await followerRepository.GetsByPropertyAsync(x => x.FollowerId == userId);
                var count = followings?.Count() ?? 0;

                var result = new FollowingCountDto
                {
                    UserId = userId,
                    FollowingCount = count
                };

                return BaseResponse<FollowingCountDto>.SuccessReturn(result, "Số lượng người đang theo dõi bạn được tính thành công.");
            }
            catch (Exception ex)
            {
                return BaseResponse<FollowingCountDto>.InternalServerError($"Lỗi khi đếm số lượng người mà bạn đang theo dõi: {ex.Message}");
            }
        }
    }

}
