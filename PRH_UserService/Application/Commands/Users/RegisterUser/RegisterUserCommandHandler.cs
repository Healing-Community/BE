using Application.Commons;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using System.Net;

namespace Application.Commands.Users.RegisterUser
{
    public class RegisterUserCommandHandler(ITokenService tokenService, IUserRepository userRepository, IEmailRepository emailRepository)
        : IRequestHandler<RegisterUserCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>() // Initialize the error list
            };

            try
            {
                // Check if passwords match
                if (request.RegisterUserDto.Password != request.RegisterUserDto.ConfirmPassword)
                {
                    response.Errors.Add("Mật khẩu và xác nhận mật khẩu không khớp.");
                }

                // Check if email already exists
                var existingUserByEmail = await userRepository.GetUserByEmailAsync(request.RegisterUserDto.Email);
                if (existingUserByEmail != null)
                {
                    response.Errors.Add("Email đã được đăng ký.");
                }

                // Check if username already exists
                var existingUserByUserName = await userRepository.GetUserByUserNameAsync(request.RegisterUserDto.UserName);
                if (existingUserByUserName != null)
                {
                    response.Errors.Add("Tên người dùng đã bị sử dụng.");
                }

                // Validate email format
                try
                {
                    var address = new System.Net.Mail.MailAddress(request.RegisterUserDto.Email);
                    if (address.Address != request.RegisterUserDto.Email)
                    {
                        response.Errors.Add("Định dạng email không hợp lệ.");
                    }
                }
                catch
                {
                    response.Errors.Add("Định dạng email không hợp lệ.");
                }

                // If there are any errors, set the response properties accordingly
                if (response.Errors.Any())
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest; // Set to BadRequest or Conflict based on your logic
                    response.Success = false;
                    return response;
                }

                // Create new user if no errors
                var user = new User
                {
                    UserId = Guid.NewGuid(),
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

                await emailRepository.SendEmailAsync(user.Email, "Xác thực email của bạn", $"Vui lòng xác thực email của bạn bằng cách nhấp vào <a href='{verificationLink}'>đây</a>.");

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
}
