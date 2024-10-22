using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.AMQP;
using AutoMapper;
using Domain.Constants;
using Domain.Constants.AMQPMessage;
using MediatR;
using NUlid;
using System.Net;

namespace Application.Commands.Report
{
    public class ReportCommandHandler(IMessagePublisher messagePublisher, IMapper mapper) : IRequestHandler<ReportCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(ReportCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>() // Khởi tạo danh sách lỗi
            };

            try
            {
                // Map ReportMessageDto sang ReportMessage
                var reportMessage = mapper.Map<ReportMessage>(request.ReportMessageDto);

                // Lấy user id từ HttpContext
                var userId = Authentication.GetUserIdFromHttpContext(request.ReportMessageDto.context ?? throw new NullReferenceException());

                // Kiểm tra nếu không có userId, báo lỗi không được phép truy cập (Unauthorized)
                if (userId == null)
                {
                    response.Success = false;
                    response.Message = "Không có quyền truy cập";
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return response;
                }

                // Gán userId vào reportMessage
                reportMessage.UserId = userId;

                // Gửi thông báo report qua hàng đợi
                await messagePublisher.PublishAsync(reportMessage, QueueName.ReportQueue, cancellationToken);

                // Thiết lập phản hồi
                response.Success = true;
                response.Message = "Báo cáo đã được gửi thành công.";
                response.StatusCode = (int)HttpStatusCode.OK;
                return response;
            }
            catch (FormatException ex)
            {
                // Bắt lỗi khi userId không hợp lệ (ví dụ không thể chuyển đổi sang GUID)
                response.Success = false;
                response.Message = "Định dạng userId không hợp lệ.";
                response.Errors.Add(ex.Message);
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                return response;
            }
            catch (Exception ex)
            {
                // Bắt lỗi tổng quát
                response.Success = false;
                response.Message = "Gửi báo cáo thất bại.";
                response.Errors.Add(ex.Message); // Thêm chi tiết lỗi vào danh sách
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                return response;
            }
        }
    }
}
