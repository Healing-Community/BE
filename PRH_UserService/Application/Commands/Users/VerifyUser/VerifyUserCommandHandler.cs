using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Users.VerifyUser
{
    public class VerifyUserCommandHandler : IRequestHandler<VerifyUserCommand, BaseResponse<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenRepository _jwtTokenRepository;

        public VerifyUserCommandHandler(IUserRepository userRepository, IJwtTokenRepository jwtTokenRepository)
        {
            _userRepository = userRepository;
            _jwtTokenRepository = jwtTokenRepository;
        }

        public async Task<BaseResponse<string>> Handle(VerifyUserCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Timestamp = DateTime.UtcNow
            };

            try
            {
                if (!_jwtTokenRepository.ValidateToken(request.Token, out Guid userId))
                {
                    response.Success = false;
                    response.Message = "Invalid or expired verification token.";
                    return response;
                }

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }

                user.Status = 1;
                await _userRepository.Update(user.Id, user);

                response.Success = true;
                response.Message = "Email verified successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to verify email.";
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }
    }
}
