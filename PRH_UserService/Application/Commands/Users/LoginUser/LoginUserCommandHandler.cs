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

        public LoginUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
                    Errors = new List<string> { "User not found." }
                };
            }

            if (!VerifyPassword(request.LoginDto.Password, user.PasswordHash))
            {
                return new BaseResponse<string>
                {
                    Success = false,
                    Message = "Invalid email or password.",
                    Errors = new List<string> { "Incorrect password." }
                };
            }

            return new BaseResponse<string>
            {
                Success = true,
                Message = "Login successful.",
                Data = "Login successful."
            };
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }
}
