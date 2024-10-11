using Application.Commons;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using System.Net;

namespace Application.Commands.Users.RegisterUser
{
    public class RegisterUserCommandHandler(ITokenService tokenService, IUserRepository userRepository, IEmailRepository emailRepository) : IRequestHandler<RegisterUserCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow
            };

            try
            {
                // Check if passwords match
                if (request.RegisterUserDto.Password != request.RegisterUserDto.ConfirmPassword)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Success = false;
                    response.Message = "Password and Confirm Password do not match.";
                    return response;
                }

                // Check if email already exists
                var existingUserByEmail = await userRepository.GetUserByEmailAsync(request.RegisterUserDto.Email);
                if (existingUserByEmail != null)
                {
                    response.StatusCode = (int)HttpStatusCode.Conflict; // Conflict for duplicate email
                    response.Success = false;
                    response.Message = "Email is already registered.";
                    return response;
                }

                // Check if username already exists
                var existingUserByUserName = await userRepository.GetUserByUserNameAsync(request.RegisterUserDto.UserName);
                if (existingUserByUserName != null)
                {
                    response.StatusCode = (int)HttpStatusCode.Conflict; // Conflict for duplicate username
                    response.Success = false;
                    response.Message = "Username is already taken.";
                    return response;
                }

                // Validate email format
                try
                {
                    var addr = new System.Net.Mail.MailAddress(request.RegisterUserDto.Email);
                    if (addr.Address != request.RegisterUserDto.Email)
                    {
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        response.Success = false;
                        response.Message = "Invalid email format.";
                        return response;
                    }
                }
                catch
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Success = false;
                    response.Message = "Invalid email format.";
                    return response;
                }

                // Create new user
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
                
                await emailRepository.SendEmailAsync(user.Email, "Verify your email", $"Please verify your email by clicking <a href='{verificationLink}'>here</a>.");

                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Registration successful. Please check your email to verify your account.";
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Failed to register user.";
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }
    }
}
