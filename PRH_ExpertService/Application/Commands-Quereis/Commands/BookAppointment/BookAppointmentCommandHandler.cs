using Application.Commons;
using Application.Commons.Tools;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Constants.AMQPMessage;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;
using UserInformation;
using UserPaymentService;

namespace Application.Commands.BookAppointment
{
    public class BookAppointmentCommandHandler(
        IGrpcHelper grpcHelper,
        IMessagePublisher messagePublisher,
        IExpertAvailabilityRepository availabilityRepository,
        IAppointmentRepository appointmentRepository,
        IExpertProfileRepository expertProfileRepository,
        IHttpContextAccessor httpContextAccessor) : IRequestHandler<BookAppointmentCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    response.Success = false;
                    response.Errors.Add("Lỗi hệ thống: không thể xác định context của yêu cầu.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                }

                var userId = Authentication.GetUserIdFromHttpContext(httpContext);
                if (string.IsNullOrEmpty(userId))
                {
                    response.Success = false;
                    response.Errors.Add("Không thể xác định UserId từ yêu cầu.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    return response;
                }

                var userEmail = Authentication.GetUserEmailFromHttpContext(httpContext);
                if (string.IsNullOrEmpty(userEmail))
                {
                    response.Errors.Add("Không tìm thấy email người dùng.");
                    response.Success = false;
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }
                try
                {
                    var userPaymentInfoReply = await grpcHelper.ExecuteGrpcCallAsync<UserService.UserServiceClient, GetUserPaymentInfoRequest, GetPaymentInfoResponse>(
                                    "UserService",
                                    async client => await client.GetUserPaymentInfoAsync(new GetUserPaymentInfoRequest { UserId = userId })
                                    );
                }
                catch
                {

                    response.Success = false;
                    response.Errors.Add("Không tìm thấy thông tin thanh toán. Vui lòng cập nhật thông tin thanh toán trước khi đặt lịch.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                var availability = await availabilityRepository.GetByIdAsync(request.ExpertAvailabilityId);
                if (availability == null || availability.Status != 0) // 0 = Available
                {
                    response.Errors.Add("Lịch trống không tồn tại hoặc không khả dụng.");
                    response.Success = false;
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                var expertProfile = await expertProfileRepository.GetByIdAsync(availability.ExpertProfileId);
                if (expertProfile == null)
                {
                    response.Errors.Add("Không tìm thấy hồ sơ chuyên gia.");
                    response.Success = false;
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                    return response;
                }

                if (expertProfile == null || expertProfile.Status != 1) // Chỉ cho phép nếu status = 1
                {
                    response.Success = false;
                    response.Errors.Add("Lịch hẹn này của chuyên gia hiện đang bị khóa và không thể đặt vào lúc này.");
                    response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                    response.StatusCode = StatusCodes.Status403Forbidden;
                    return response;
                }

                var expertEmail = expertProfile.Email;

                // Đổi trạng thái của lịch trống sang PendingPayment
                availability.Status = 1; // PendingPayment
                availability.UpdatedAt = DateTime.UtcNow.AddHours(7);
                await availabilityRepository.Update(availability.ExpertAvailabilityId, availability);

                // Tạo liên kết Jitsi Meet
                var meetingRoomName = $"{Ulid.NewUlid()}-{availability.ExpertProfileId}";
                var meetingUrl = $"https://meet.jit.si/{meetingRoomName}";

                // Tạo Appointment
                var appointment = new Appointment
                {
                    AppointmentId = Ulid.NewUlid().ToString(),
                    UserId = userId,
                    UserEmail = userEmail,
                    ExpertProfileId = availability.ExpertProfileId,
                    ExpertEmail = expertEmail,
                    ExpertAvailabilityId = availability.ExpertAvailabilityId,
                    AppointmentDate = availability.AvailableDate,
                    StartTime = availability.StartTime,
                    EndTime = availability.EndTime,
                    Status = 0, // PendingPayment
                    MeetLink = meetingUrl,
                    CreatedAt = DateTime.UtcNow.AddHours(7),
                    UpdatedAt = DateTime.UtcNow.AddHours(7)
                };

                await appointmentRepository.Create(appointment);

                response.Success = true;
                response.Data = appointment.AppointmentId;
                response.StatusCode = StatusCodes.Status200OK;
                response.Message = "Yêu cầu đặt lịch đã được ghi nhận. Vui lòng hoàn tất thanh toán để xác nhận lịch hẹn.";

                // send mail to expert
                var expertMail = new SendMailMessage
                {
                    To = expertEmail,
                    Subject = "Có lịch hẹn mới",
                    Body = $@"
<html>
<body style=""margin: 0; padding: 0; font-family: 'Verdana', sans-serif; background-color: #f0f4f8;"">
    <div style=""max-width: 650px; margin: 0 auto; background-color: #fff; padding: 30px; border-radius: 10px; box-shadow: 0 6px 20px rgba(0, 0, 0, 0.1);"">
        <div style=""text-align: center;"">
            <img src=""https://firebasestorage.googleapis.com/v0/b/healing-community.appspot.com/o/logo%2Flogo.png?alt=media&token=4e7cda70-2c98-4185-a693-b03564f68a4c"" alt=""Healing Image"" style=""max-width: 100%; height: auto; border-radius: 8px;"">
        </div>
        <h2 style=""color: #4caf50; text-align: center; margin-top: 20px;"">Lịch hẹn mới</h2>
        <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
            Bạn có một lịch hẹn mới vào lúc <strong>{availability.StartTime}</strong>, ngày <strong>{availability.AvailableDate}</strong>. 
        </p>
        <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
            Vui lòng kiểm tra và xác nhận lịch hẹn để đảm bảo mọi thứ được chuẩn bị tốt nhất.
        </p>
        <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
            Cảm ơn bạn đã tin tưởng và sử dụng dịch vụ của <strong>Healing Community</strong>.
        </p>
        <p style=""text-align: center; color: #999; font-size: 13px;"">&copy; 2024 Healing Community. Tất cả các quyền được bảo lưu.</p>
    </div>
</body>
</html>
"
                };
                await messagePublisher.PublishAsync(expertMail, QueueName.MailQueue, cancellationToken);
                // Mail cho người dùng
                var userMail = new SendMailMessage
                {
                    To = userEmail,
                    Subject = "Đặt lịch hẹn thành công",
                    Body = $@"
    <html>
    <body style=""margin: 0; padding: 0; font-family: 'Verdana', sans-serif; background-color: #f0f4f8;"">
        <div style=""max-width: 650px; margin: 0 auto; background-color: #fff; padding: 30px; border-radius: 10px; box-shadow: 0 6px 20px rgba(0, 0, 0, 0.1);"">
            <div style=""text-align: center;"">
                <img src=""https://firebasestorage.googleapis.com/v0/b/healing-community.appspot.com/o/logo%2Flogo.png?alt=media&token=4e7cda70-2c98-4185-a693-b03564f68a4c"" alt=""Healing Image"" style=""max-width: 100%; height: auto; border-radius: 8px;"">
            </div>
            <h2 style=""color: #4caf50; text-align: center; margin-top: 20px;"">Đặt lịch hẹn thành công</h2>
            <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
                Xin chúc mừng! Bạn đã đặt lịch hẹn thành công với thông tin sau:
            </p>
            <ul style=""font-size: 17px; line-height: 1.8; color: #444; padding-left: 20px;"">
                <li><strong>Thời gian:</strong> {availability.StartTime}</li>
                <li><strong>Ngày:</strong> {availability.AvailableDate}</li>
            </ul>
            <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
                Vui lòng kiểm tra lại thông tin trên và có mặt đúng giờ để đảm bảo buổi hẹn diễn ra suôn sẻ. Nếu bạn có bất kỳ thay đổi nào cần cập nhật, vui lòng liên hệ với chúng tôi qua email hoặc hotline.
            </p>
            <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
            Nếu bạn chưa thanh toán thì có thể bỏ qua email này, Link thanh toán sẽ hết hạn sau 5 phút.
            </p>
            <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">
                Cảm ơn bạn đã tin tưởng và sử dụng dịch vụ của <strong>Healing Community</strong>. Chúng tôi hy vọng sẽ mang lại cho bạn trải nghiệm tốt nhất.
            </p>
            <p style=""text-align: center; color: #999; font-size: 13px;"">&copy; 2024 Healing Community. Tất cả các quyền được bảo lưu.</p>
        </div>
    </body>
    </html>
    "
                };
                await messagePublisher.PublishAsync(userMail, QueueName.MailQueue, cancellationToken);
                

            }
            catch (Exception ex)
            {
                // Xử lý lỗi hệ thống
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
                response.Success = false;
                response.Message = string.Join(" ", response.Errors); // Gộp lỗi vào Message
                response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            return response;
        }
    }
}
