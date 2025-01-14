using Application.Commons.DTOs;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commads_Queries.Queries.Posts.GetCountPosts
{
    public class CountPostsQueryHandler(IPostRepository postRepository)
    : IRequestHandler<CountPostsQuery, BaseResponse<PostCountDto>>
    {
        public async Task<BaseResponse<PostCountDto>> Handle(CountPostsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = request.UserId;

                // Đếm số bài viết mà user đã đăng
                var postsByUser = await postRepository.GetByUserIdAsync(userId);

                var totalPosts = postsByUser?.Count() ?? 0;

                var result = new PostCountDto
                {
                    UserId = userId,
                    PostCount = totalPosts
                };

                return BaseResponse<PostCountDto>.SuccessReturn(result, "Tổng số bài viết của người dùng đã được tính thành công.");
            }
            catch (Exception ex)
            {
                return BaseResponse<PostCountDto>.InternalServerError($"Lỗi khi đếm số bài viết: {ex.Message}");
            }
        }
    }


}
