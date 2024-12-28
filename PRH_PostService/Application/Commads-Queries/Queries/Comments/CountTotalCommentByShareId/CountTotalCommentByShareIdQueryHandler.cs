using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NUlid;
using System.Net;

namespace Application.Commads_Queries.Queries.Comments.CountTotalCommentByShareId
{
    public class CountTotalCommentByShareIdQueryHandler : IRequestHandler<CountTotalCommentByShareIdQuery, BaseResponse<object>>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IShareRepository _shareRepository;

        public CountTotalCommentByShareIdQueryHandler(ICommentRepository commentRepository, IShareRepository shareRepository)
        {
            _commentRepository = commentRepository;
            _shareRepository = shareRepository;
        }

        public async Task<BaseResponse<object>> Handle(CountTotalCommentByShareIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<object>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                // Kiểm tra ShareId có tồn tại không
                var shareExists = await _shareRepository.ExistsAsync(request.ShareId);
                if (!shareExists)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy bài chia sẻ để đếm bình luận.";
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    return response;
                }

                // Đếm số lượng bình luận qua ShareId
                var totalComments = await _commentRepository.GetQueryable()
                    .Where(c => c.ShareId == request.ShareId)
                    .CountAsync(cancellationToken);

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
