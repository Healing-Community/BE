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
        private readonly IJwtTokenRepository _jwtTokenRepository;
        private readonly IEmailRepository _emailRepository;

        public RegisterUserCommandHandler(IUserRepository userRepository, IJwtTokenRepository jwtTokenRepository, IEmailRepository emailRepository)
        {
            _userRepository = userRepository;
            _jwtTokenRepository = jwtTokenRepository;
            _emailRepository = emailRepository;
        }

        public async Task<BaseResponse<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (request.RegisterUserDto.Password != request.RegisterUserDto.ConfirmPassword)
            {
                return new BaseResponse<string>
                {
                    Success = false,
                    Message = "Password and Confirm Password do not match.",
                    Timestamp = DateTime.UtcNow
                };
            }

            var existingUser = await _userRepository.GetUserByEmailAsync(request.RegisterUserDto.Email);
            if (existingUser != null)
            {
                return new BaseResponse<string>
                {
                    Success = false,
                    Message = "Email is already registered.",
                    Timestamp = DateTime.UtcNow
                };
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.RegisterUserDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.RegisterUserDto.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = 0,
                RoleId = 4
            };

            await _userRepository.Create(user);

            var verificationToken = _jwtTokenRepository.GenerateVerificationToken(user);
            var verificationLink = $"https://healingcommunity.com/verify?token={verificationToken}";

            await _emailRepository.SendEmailAsync(user.Email, "Verify your email", $"Please verify your email by clicking <a href='{verificationLink}'>here</a>.");

            return new BaseResponse<string>
            {
                Success = true,
                Message = "Registration successful. Please check your email to verify your account.",
                Data = null,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
