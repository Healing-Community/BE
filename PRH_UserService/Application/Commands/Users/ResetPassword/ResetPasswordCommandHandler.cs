using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Users.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, BaseResponse<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenRepository _jwtTokenRepository;

        public ResetPasswordCommandHandler(IUserRepository userRepository, IJwtTokenRepository jwtTokenRepository)
        {
            _userRepository = userRepository;
            _jwtTokenRepository = jwtTokenRepository;
        }

        public async Task<BaseResponse<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow
            };

            try
            {
                if (request.ResetPasswordDto.NewPassword != request.ResetPasswordDto.ConfirmPassword)
                {
                    response.Success = false;
                    response.Message = "Passwords do not match.";
                    return response;
                }

                if (!_jwtTokenRepository.ValidateToken(request.ResetPasswordDto.Token, out Guid userId))
                {
                    response.Success = false;
                    response.Message = "Invalid or expired token.";
                    return response;
                }

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.ResetPasswordDto.NewPassword);
                await _userRepository.Update(user.UserId, user);

                response.Success = true;
                response.Message = "Password reset successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to reset password.";
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }
    }
}
