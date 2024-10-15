using System.Security.Claims;
using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using Application.Interfaces.Services;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Application.Commands.Users.LoginUser;

public class LoginUserCommandHandler(
    IConfiguration configuration,
    ITokenRepository tokenRepository,
    IRoleRepository roleRepository,
    ITokenService tokenService,
    IUserRepository userRepository)
    : IRequestHandler<LoginUserCommand, BaseResponse<TokenDto>>
{
    public async Task<BaseResponse<TokenDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<TokenDto>
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow
        };

        try
        {
            var user = await userRepository.GetUserByEmailAsync(request.LoginDto.Email);
            if (user == null)
                return new BaseResponse<TokenDto>
                {
                    Id = Guid.NewGuid(),
                    Success = false,
                    Message = "Invalid email or password.",
                    Errors = new List<string> { "User not found." },
                    Timestamp = DateTime.UtcNow,
                    StatusCode = (int)StatusCodes.Status401Unauthorized
                };

            if (user.Status == 0)
                return new BaseResponse<TokenDto>
                {
                    Id = Guid.NewGuid(),
                    Success = false,
                    Message = "User account is inactive.",
                    Errors = new List<string> { "Inactive account." },
                    Timestamp = DateTime.UtcNow,
                    StatusCode = (int)StatusCodes.Status401Unauthorized
                };

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.LoginDto.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                if (request.LoginDto.Password == user.PasswordHash)
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.LoginDto.Password);
                    await userRepository.Update(user.UserId, user);
                    isPasswordValid = true;
                }
                else
                {
                    return new BaseResponse<TokenDto>
                    {
                        Id = Guid.NewGuid(),
                        Success = false,
                        Message = "Invalid email or password.",
                        Errors = new List<string> { "Incorrect password." },
                        Timestamp = DateTime.UtcNow,
                        StatusCode = (int)StatusCodes.Status401Unauthorized
                    };
                }
            }

            var roleName = roleRepository.GetRoleNameById(user.RoleId);
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, roleName.Result ?? "None")
            };

            var accessToken = tokenService.GenerateAccessToken(claims);
            response.Success = true;
            response.Message = "Login successful.";

            var tokenData = new TokenDto
            {
                Token = accessToken,
                RefreshToken = tokenService.GenerateRefreshToken()
            };

            response.Data = tokenData;

            // Save Refresh token into database

            var userToken = await tokenRepository.GetByPropertyAsync(t => t.UserId == user.UserId);

            if (userToken != null) await tokenRepository.DeleteAsync(userToken.UserId);

            var token = new Token
            {
                UserId = user.UserId,
                RefreshToken = tokenData.RefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(configuration["JwtSettings:ExpiryMinutes"] ?? "60"))
            };
            await tokenRepository.Create(token);
            response.StatusCode = (int)StatusCodes.Status200OK;
        }
        catch (Exception ex)
        {
            response.StatusCode = (int)StatusCodes.Status500InternalServerError;
            response.Success = false;
            response.Message = "Failed to login user.";
            response.Errors = new List<string> { ex.Message };
        }

        return response;
    }
}
