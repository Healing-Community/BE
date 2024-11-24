using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System.Net;

namespace Application.Queries.Comments.CountTotalCommentByPostId
{
    public class CountTotalCommentByPostIdQueryHandler : IRequestHandler<CountTotalCommentByPostIdQuery, BaseResponse<object>>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository; 

        public CountTotalCommentByPostIdQueryHandler(ICommentRepository commentRepository, IPostRepository postRepository)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
        }

        public async Task<BaseResponse<object>> Handle(CountTotalCommentByPostIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<object>
            {
                Id = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };

            try
            {
                // Kiểm tra PostId có tồn tại không
                var postExists = await _postRepository.ExistsAsync(request.PostId);
                if (!postExists)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy bài viết để đếm.";
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    return response;
                }

                // Đếm số lượng comment qua hàm CountCommentsByPostIdAsync
                var totalComments = await _commentRepository.CountCommentsByPostIdAsync(request.PostId);

                // Tạo object trả về
                var result = new
                {
                    countTotalComment = totalComments
                };

                response.Data = result;
                response.Success = true;
                response.Message = "Đếm tổng số lượng bình luận thành công.";
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi đếm bình luận.";
                response.Errors.Add(ex.Message);
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return response;
        }
    }
}
