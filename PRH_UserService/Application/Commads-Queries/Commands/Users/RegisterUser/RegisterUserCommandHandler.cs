using System.Net;
using System.Net.Mail;
using Application.Commons;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Commands.Users.RegisterUser;

public class RegisterUserCommandHandler(
    ITokenService tokenService,
    IUserRepository userRepository,
    IEmailRepository emailRepository)
    : IRequestHandler<RegisterUserCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<string>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.UtcNow,
            Errors = new List<string>() // Initialize the error list
        };

        try
        {
            // Check if passwords match
            if (request.RegisterUserDto.Password != request.RegisterUserDto.ConfirmPassword)
                response.Errors.Add("Mật khẩu và xác nhận mật khẩu không khớp.");

            // Check if email already exists
            var existingUserByEmail = await userRepository.GetUserByEmailAsync(request.RegisterUserDto.Email);
            if (existingUserByEmail != null) response.Errors.Add("Email đã được đăng ký.");

            // Check if username already exists
            var existingUserByUserName = await userRepository.GetUserByUserNameAsync(request.RegisterUserDto.UserName);
            if (existingUserByUserName != null) response.Errors.Add("Tên người dùng đã bị sử dụng.");

            // Validate email format
            try
            {
                var address = new MailAddress(request.RegisterUserDto.Email);
                if (address.Address != request.RegisterUserDto.Email)
                    response.Errors.Add("Định dạng email không hợp lệ.");
            }
            catch
            {
                response.Errors.Add("Định dạng email không hợp lệ.");
            }

            // If there are any errors, set the response properties accordingly
            if (response.Errors.Any())
            {
                response.StatusCode =
                    (int)HttpStatusCode.BadRequest; // Set to BadRequest or Conflict based on your logic
                response.Success = false;
                return response;
            }

            // Create new user if no errors
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

            // Generate verification token and send email
            var verificationToken = tokenService.GenerateVerificationToken(user);
            var verificationLink = $"{request.BaseUrl}/api/user/verify-user?Token={verificationToken}";

            // Prepare email content
            string emailContent = $@"
                <html>
                <body style=""margin: 0; padding: 0; font-family: 'Verdana', sans-serif; background-color: #f0f4f8;"">
                    <div style=""max-width: 650px; margin: 0 auto; background-color: #fff; padding: 30px; border-radius: 10px; box-shadow: 0 6px 20px rgba(0, 0, 0, 0.1);"">
                        <div style=""text-align: center;"">
                            <img src=""https://i.postimg.cc/zXN0D5kY/logo.png"" alt=""Healing Image"" style=""max-width: 100%; height: auto; border-radius: 8px;"">
                        </div>
                        <h2 style=""color: #4caf50; text-align: center; margin-top: 20px;"">Chào mừng bạn đến với Cộng đồng Chữa lành!</h2>
                        <p style=""font-size: 17px; line-height: 1.8; color: #444; text-align: justify;"">Cảm ơn bạn đã tin tưởng và đăng ký với chúng tôi. Hãy xác thực tài khoản của bạn bằng cách nhấn vào liên kết dưới đây để bắt đầu hành trình chữa lành của bạn.</p>
                        <div style=""text-align: center; margin-top: 20px;"">
                            <a href=""{verificationLink}"" style=""background-color: #4caf50; color: #ffffff; padding: 12px 30px; border-radius: 5px; text-decoration: none; font-size: 18px;"">Xác thực Email</a>
                        </div>
                        <p style=""font-size: 14px; color: #666; text-align: center; margin-top: 30px;"">Nếu bạn không yêu cầu đăng ký, vui lòng bỏ qua email này.</p>
                        <p style=""text-align: center; color: #999; font-size: 13px;"">&copy; 2024 Healing Community. Tất cả các quyền được bảo lưu.</p>
                    </div>
                </body>
                </html>
                ";

            // Send verification email
            await emailRepository.SendEmailAsync(user.Email, "Xác thực email của bạn", emailContent);

            response.StatusCode = (int)HttpStatusCode.OK;
            response.Success = true;
            response.Message = "Đăng ký thành công. Vui lòng kiểm tra email của bạn để xác thực tài khoản.";
        }
        catch (Exception ex)
        {
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            response.Success = false;
            response.Message = "Đã xảy ra lỗi khi đăng ký người dùng.";
            response.Errors.Add(ex.Message); // Add the exception message to the error list
        }

        return response;
    }
}