using Application.Commons;
using Application.Commons.Tools;
using Domain.Entities.DASS21;
using MassTransit;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries.GetDass21Result
{
    public class GetDass21ResultQueryHandler : IRequestHandler<GetDass21ResultQuery, BaseResponse<Dass21Result>>
    {
        private readonly IMongoRepository<Dass21Result> _mongoRepository;

        public GetDass21ResultQueryHandler(IMongoRepository<Dass21Result> mongoRepository)
        {
            _mongoRepository = mongoRepository ?? throw new ArgumentNullException(nameof(mongoRepository));
        }

        public async Task<BaseResponse<Dass21Result>> Handle(GetDass21ResultQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<Dass21Result>
            {
                Id = NewId.NextSequentialGuid(),
                Timestamp = DateTime.UtcNow
            };

            try
            {
                // Kiểm tra xem HttpContext có hợp lệ không
                if (request.HttpContext == null)
                {
                    response.Errors.Add("HttpContext is null");
                    response.Message = "Không xác định được thông tin HttpContext";
                    response.StatusCode = 400;
                    return response;
                }

                // Lấy UserId từ HttpContext và kiểm tra giá trị
                var userId = Authentication.GetUserIdFromHttpContext(request.HttpContext);
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                {
                    response.Errors.Add("UserId không hợp lệ");
                    response.Message = "Không thể xác định UserId từ HttpContext";
                    response.StatusCode = 400;
                    return response;
                }

                // Tìm kết quả DASS-21 của người dùng theo UserId
                var result = await _mongoRepository.GetByPropertyAsync(x => x.UserId == userGuid);
                if (result == null)
                {
                    response.Errors.Add("Không tìm thấy kết quả");
                    response.Message = "Không có kết quả nào cho UserId này";
                    response.StatusCode = 404;
                    return response;
                }

                // Thiết lập phản hồi thành công
                response.Data = result;
                response.Success = true;
                response.StatusCode = 200;
                response.Message = "Lấy kết quả thành công";
            }
            catch
            {
                response.Success = false;
                response.StatusCode = 500;
                response.Message = "Đã xảy ra lỗi khi lấy kết quả DASS-21";
            }

            return response;
        }
    }
}
