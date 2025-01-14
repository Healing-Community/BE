using Application.Commons.DTOs;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commands_Queries.Queries.Users.GetCountUserFollower
{
    public class CountFollowersQueryHandler(IFollowerRepository followerRepository)
        : IRequestHandler<CountFollowersQuery, BaseResponse<FollowerCountDto>>
    {
        public async Task<BaseResponse<FollowerCountDto>> Handle(CountFollowersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = request.UserId;
                var followers = await followerRepository.GetsByPropertyAsync(x => x.UserId == userId);
                var count = followers?.Count() ?? 0;

                var result = new FollowerCountDto
                {
                    UserId = userId,
                    FollowerCount = count
                };

                return BaseResponse<FollowerCountDto>.SuccessReturn(result, "Số lượng người bạn đang theo dõi được tính thành công.");
            }
            catch (Exception ex)
            {
                return BaseResponse<FollowerCountDto>.InternalServerError($"Lỗi khi đếm số lượng người theo dõi: {ex.Message}");
            }
        }
    }

}
