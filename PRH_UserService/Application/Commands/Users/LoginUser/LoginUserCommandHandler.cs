using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Users.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, BaseResponse<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenRepository _jwtTokenRepository;

        public LoginUserCommandHandler(IUserRepository userRepository, IJwtTokenRepository jwtTokenRepository)
        {
            _userRepository = userRepository;
            _jwtTokenRepository = jwtTokenRepository;
        }

        public async Task<BaseResponse<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.LoginDto.Email);
            if (user == null)
            {
                return new BaseResponse<string>
                {
                    Success = false,
                    Message = "Invalid email or password.",
                    Errors = new List<string> { "User not found." },
                    Timestamp = DateTime.UtcNow
                };
            }

            if (!VerifyPassword(request.LoginDto.Password, user.PasswordHash))
            {
                return new BaseResponse<string>
                {
                    Success = false,
                    Message = "Invalid email or password.",
                    Errors = new List<string> { "Incorrect password." },
                    Timestamp = DateTime.UtcNow
                };
            }

            var token = _jwtTokenRepository.GenerateToken(user);

            return new BaseResponse<string>
            {
                Success = true,
                Message = "Login successful.",
                Data = token,
                Timestamp = DateTime.UtcNow
            };
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }
}
