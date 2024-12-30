using Application.Commons.DTOs;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;


namespace Application.Commads_Queries.Queries.Share.CountShareByPostId
{
    public class CountShareByPostIdQueryHandler(IShareRepository shareRepository, IPostRepository postRepository)
            : IRequestHandler<CountShareByPostIdQuery, BaseResponse<ShareCountDto>>
    {
        public async Task<BaseResponse<ShareCountDto>> Handle(CountShareByPostIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Kiểm tra bài viết có tồn tại hay không
                var post = await postRepository.GetByPropertyAsync(x => x.PostId == request.PostId.PostId);
                if (post == null)
                {
                    return BaseResponse<ShareCountDto>.NotFound("Bài viết không tồn tại.");
                }

                // Đếm số lượt chia sẻ của bài viết
                var shareCount = await shareRepository.CountByPostIdAsync(request.PostId.PostId);

                // Tạo DTO kết quả
                var result = new ShareCountDto
                {
                    PostId = request.PostId.PostId,
                    ShareCount = shareCount
                };

                return BaseResponse<ShareCountDto>.SuccessReturn(result, "Đếm số lượt chia sẻ thành công.");
            }
            catch (Exception ex)
            {
                return BaseResponse<ShareCountDto>.InternalServerError($"Đã xảy ra lỗi: {ex.Message}");
            }
        }
    }
}
