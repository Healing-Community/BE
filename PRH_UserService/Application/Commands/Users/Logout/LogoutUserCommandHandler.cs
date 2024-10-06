using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Enum;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Users.Logout
{
    public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand, BaseResponse<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenRepository _jwtTokenRepository;

        public LogoutUserCommandHandler(IUserRepository userRepository, IJwtTokenRepository jwtTokenRepository)
        {
            _userRepository = userRepository;
            _jwtTokenRepository = jwtTokenRepository;
        }

        public async Task<BaseResponse<string>> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.UtcNow
            };

            try
            {
                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    response.Errors = new[] { "User not found." };
                    return response;
                }

                if (user.Tokens.Any())
                {
                    foreach (var token in user.Tokens)
                    {
                        token.Status = (int)TokenStatus.Revoked;
                    }
                    await _userRepository.Update(user.UserId, user);
                }

                response.Success = true;
                response.Message = "User logged out successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while logging out.";
                response.Errors = new[] { ex.Message };
            }

            return response;
        }
    }
}
