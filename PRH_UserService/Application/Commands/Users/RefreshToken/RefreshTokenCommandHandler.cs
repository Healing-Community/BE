using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Users.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, BaseResponse<string>>
    {
        private readonly IJwtTokenRepository _jwtTokenRepository;
        private readonly IUserRepository _userRepository;

        public RefreshTokenCommandHandler(IJwtTokenRepository jwtTokenRepository, IUserRepository userRepository)
        {
            _jwtTokenRepository = jwtTokenRepository;
            _userRepository = userRepository;
        }

        public async Task<BaseResponse<string>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow
            };

            try
            {
                if (!_jwtTokenRepository.ValidateRefreshToken(request.RefreshToken, out Guid userId))
                {
                    response.Success = false;
                    response.Message = "Invalid or expired refresh token.";
                    return response;
                }

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }

                var token = user.Tokens.FirstOrDefault(t => t.RefreshTokenHash == request.RefreshToken);
                if (token == null || token.ExpiresAt < DateTime.UtcNow)
                {
                    response.Success = false;
                    response.Message = "Expired refresh token.";
                    return response;
                }

                var newToken = _jwtTokenRepository.GenerateToken(user);
                response.Success = true;
                response.Message = "Token refreshed successfully.";
                response.Data = newToken;

                token.IsUsed = true;
                await _userRepository.Update(user.UserId, user);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to refresh token.";
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }
    }
}
