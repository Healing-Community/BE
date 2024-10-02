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
            if (!_jwtTokenRepository.ValidateToken(request.Token, out Guid userId))
            {
                return new BaseResponse<string>
                {
                    Success = false,
                    Message = "Invalid or expired verification token.",
                    Timestamp = DateTime.UtcNow
                };
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new BaseResponse<string>
                {
                    Success = false,
                    Message = "User not found.",
                    Timestamp = DateTime.UtcNow
                };
            }

            user.Status = 1;
            await _userRepository.Update(user.Id, user);

            return new BaseResponse<string>
            {
                Success = true,
                Message = "Email verified successfully.",
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
