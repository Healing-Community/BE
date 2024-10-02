﻿using Application.Commons;
using Application.Interfaces.Repository;
using BCrypt.Net;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
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
            var response = new BaseResponse<string>
            {
                Timestamp = DateTime.UtcNow
            };

            try
            {
                var user = await _userRepository.GetUserByEmailAsync(request.LoginDto.Email);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Invalid email or password.";
                    response.Errors = new List<string> { "User not found." };
                    return response;
                }

                bool isPasswordValid = false;
                try
                {
                    isPasswordValid = BCrypt.Net.BCrypt.Verify(request.LoginDto.Password, user.PasswordHash);
                }
                catch (SaltParseException)
                {                  
                    if (request.LoginDto.Password == user.PasswordHash)
                    {
                        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.LoginDto.Password);
                        await _userRepository.Update(user.Id, user);
                        isPasswordValid = true;
                    }
                }

                if (!isPasswordValid)
                {
                    response.Success = false;
                    response.Message = "Invalid email or password.";
                    response.Errors = new List<string> { "Incorrect password." };
                    return response;
                }

                var token = _jwtTokenRepository.GenerateToken(user);
                response.Success = true;
                response.Message = "Login successful.";
                response.Data = token;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failed to login user.";
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }
    }
}
