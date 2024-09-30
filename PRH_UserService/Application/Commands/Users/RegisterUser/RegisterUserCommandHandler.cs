using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using Domain.Enum;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Users.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, BaseResponse<string>>
    {
        private readonly IUserRepository _userRepository;

        public RegisterUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<BaseResponse<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var existingUserByUserName = await _userRepository.GetUserByUserNameAsync(request.RegisterUserDto.UserName);
            if (existingUserByUserName != null)
            {
                return new BaseResponse<string>
                {
                    Success = false,
                    Message = "UserName already exists.",
                    Data = null
                };
            }

            var existingUserByEmail = await _userRepository.GetUserByEmailAsync(request.RegisterUserDto.Email);
            if (existingUserByEmail != null)
            {
                return new BaseResponse<string>
                {
                    Success = false,
                    Message = "Email already exists.",
                    Data = null
                };
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = request.RegisterUserDto.FullName,
                UserName = request.RegisterUserDto.UserName,
                Email = request.RegisterUserDto.Email,
                PasswordHash = HashPassword(request.RegisterUserDto.Password),
                Status = (int)UserStatus.Active,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                RoleId = 4
            };

            await _userRepository.Create(user);

            return new BaseResponse<string>
            {
                Success = true,
                Message = "Registration successful.",
                Data = "User registered successfully."
            };
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
