using System.Net;
using System.Net.Mail;
using Application.Commons;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands.Users.RegisterUser;

public class RegisterUserCommandHandler(
    ITokenService tokenService,
    IUserRepository userRepository,
    IEmailService emailService)
    : IRequestHandler<RegisterUserCommand, DetailBaseResponse<string>>
{
    public async Task<DetailBaseResponse<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var response = new DetailBaseResponse<string>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.UtcNow,
            Errors = new List<ErrorDetail>() // Khởi tạo danh sách lỗi theo định dạng yêu cầu
        };

        try
        {
            // Bắt lỗi cho mật khẩu
            if (request.RegisterUserDto.Password.Length < 8)
            {
                response.Errors.Add(new ErrorDetail
                {
                    Message = "Mật khẩu phải có ít nhất 8 ký tự.",
                    Field = "password"
                });
            }

            if (request.RegisterUserDto.Password != request.RegisterUserDto.ConfirmPassword)
            {
                response.Errors.Add(new ErrorDetail
                {
                    Message = "Mật khẩu và xác nhận mật khẩu không khớp.",
                    Field = "re-password"
                });
            }

            // Kiểm tra xem email đã được đăng ký hay chưa
            var existingUser = await userRepository.GetByPropertyAsync(u => u.Email == request.RegisterUserDto.Email);
            if (existingUser != null)
            {
                // Nếu tài khoản tồn tại nhưng chưa được xác minh, xóa tài khoản đó
                if (existingUser.Status == 0)
                {
                    await userRepository.DeleteAsync(existingUser.UserId);
                }
                else
                {
                    // Nếu tài khoản đã được xác minh, thông báo lỗi
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Email đã được đăng ký.",
                        Field = "email"
                    });
                }
            }

            // Kiểm tra xem tên người dùng đã tồn tại hay chưa
            var existingUserByUserName = await userRepository.GetUserByUserNameAsync(request.RegisterUserDto.UserName);
            if (existingUserByUserName != null)
            {
                response.Errors.Add(new ErrorDetail
                {
                    Message = "Tên người dùng đã bị sử dụng.",
                    Field = "username"
                });
            }

            // Bắt lỗi cho username
            if (request.RegisterUserDto.UserName.Length < 8)
            {
                response.Errors.Add(new ErrorDetail
                {
                    Message = "Tên người dùng phải có ít nhất 8 ký tự.",
                    Field = "username"
                });
            }

            if (request.RegisterUserDto.UserName.Length > 20)
            {
                response.Errors.Add(new ErrorDetail
                {
                    Message = "Tên người dùng không được quá 20 ký tự.",
                    Field = "username"
                });
            }

            // Kiểm tra định dạng email hợp lệ
            try
            {
                var address = new MailAddress(request.RegisterUserDto.Email);
                if (address.Address != request.RegisterUserDto.Email)
                {
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Định dạng email không hợp lệ.",
                        Field = "email"
                    });
                }
            }
            catch
            {
                response.Errors.Add(new ErrorDetail
                {
                    Message = "Định dạng email không hợp lệ.",
                    Field = "email"
                });
            }

            // Nếu có bất kỳ lỗi nào, trả về BadRequest và hiển thị các lỗi
            if (response.Errors.Any())
            {
                response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                response.Success = false;
                response.Message = "Đã xảy ra lỗi trong quá trình đăng ký.";
                return response;
            }

            // Tạo người dùng mới nếu không có lỗi
            var user = new User
            {
                UserId = Ulid.NewUlid().ToString(),
                Email = request.RegisterUserDto.Email,
                UserName = request.RegisterUserDto.UserName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.RegisterUserDto.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = 0,
                RoleId = 1
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
