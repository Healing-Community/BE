using System.Net;
using Application.Commons;
using Application.Commons.Enum;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Constants.AMQPMessage;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands_Queries.Commands.Users.RegisterUser;

public class RegisterUserCommandHandler(
    ITokenService tokenService,
    IUserRepository userRepository,
    IEmailService emailService,
    IMessagePublisher messagePublisher)
    : IRequestHandler<RegisterUserCommand, DetailBaseResponse<string>>
{
    public async Task<DetailBaseResponse<string>> Handle(RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var response = new DetailBaseResponse<string>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.UtcNow,
            Errors = new List<ErrorDetail>() // Khởi tạo danh sách lỗi theo định dạng yêu cầu
        };

        try
        {
            // Kiểm tra xem email đã được đăng ký hay chưa
            var existingUser = await userRepository.GetByPropertyAsync(u => u.Email == request.RegisterUserDto.Email);
            if (existingUser != null)
            {
                // Nếu tài khoản tồn tại nhưng chưa được xác minh, xóa tài khoản đó
                if (existingUser.Status == 0)
                    await userRepository.DeleteAsync(existingUser.UserId);
                else
                    // Nếu tài khoản đã được xác minh, thông báo lỗi
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Email đã được đăng ký.",
                        Field = "email"
                    });
            }

            // Kiểm tra xem tên người dùng đã tồn tại hay chưa
            var existingUserByUserName = await userRepository.GetUserByUserNameAsync(request.RegisterUserDto.UserName);
            if (existingUserByUserName != null)
                response.Errors.Add(new ErrorDetail
                {
                    Message = "Tên người dùng đã bị sử dụng.",
                    Field = "username"
                });
            // Nếu có bất kỳ lỗi nào, trả về BadRequest và hiển thị các lỗi
            if (response.Errors.Count != 0)
            {
                response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                response.Success = false;
                response.Message = "Đã xảy ra lỗi trong quá trình đăng ký.";
                return response;
            }
            var user = new User
            {
                UserId = Ulid.NewUlid().ToString(),
                Email = request.RegisterUserDto.Email,
                UserName = request.RegisterUserDto.UserName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.RegisterUserDto.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = 0,
                // Mặc định là người dùng thường (role = 1), nếu đăng ký là chuyên gia thì role = 2
                RoleId = request.RegisterUserDto.IsExpert ? (int)EnumRole.Expert : (int)EnumRole.User
            };

            await userRepository.Create(user);

            // Tạo token xác minh và gửi email xác nhận
            var verificationToken = tokenService.GenerateVerificationToken(user);
            var verificationLink = $"{request.BaseUrl}/api/user/verify-user?Token={verificationToken}";

            // Gửi email xác minh
            await emailService.SendVerificationEmailAsync(user.Email, verificationLink);
            // Trả về thông báo thành công
            response.StatusCode = (int)HttpStatusCode.OK;
            response.Success = true;
            response.Message = "Đăng ký thành công. Vui lòng kiểm tra email của bạn để xác thực tài khoản.";

            if (!request.RegisterUserDto.IsExpert) return response;
            // Publish message to RabbitMQ
            await messagePublisher.PublishAsync(new CreateExpertMessage{
                UserId = user.UserId,
                UserName = user.UserName
            },QueueName.ExpertCreateQueue, cancellationToken);
        }
        catch (Exception ex)
        {
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            response.Success = false;
            response.Message = "Đã xảy ra lỗi khi đăng ký người dùng.";
            response.Errors.Add(new ErrorDetail
            {
                Message = ex.Message,
                Field = "exception"
            });
        }

        return response;
    }
}