using System;
using System.Security.Claims;
using Application.Commons;
using Application.Interfaces.Repository;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands_Queries.Commands.Users.UpdateUserProfile;

public class UpdateUserProfileCommandHandler(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor) : IRequestHandler<UpdateUserProfileCommand, DetailBaseResponse<string>>
{
    public async Task<DetailBaseResponse<string>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return new DetailBaseResponse<string>
                {
                    Id = NewId.NextGuid().ToString(),
                    Message = "Không tìm thấy thông tin người dùng từ token",
                    Success = false
                };
            }


            var user = await userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new DetailBaseResponse<string>
                {
                    Id = NewId.NextGuid().ToString(),
                    Message = "Không tìm thấy thông tin người dùng",
                    Success = false
                };
            }

            user.FullName = request.UserDto.FullName;
            user.PhoneNumber = request.UserDto.PhoneNumber;
            user.Descrtiption = request.UserDto.Descrtiption;

            await userRepository.UpdateAsync(userId, user);
        }
        catch
        {
            return new DetailBaseResponse<string>
            {
                Id = NewId.NextGuid().ToString(),
                Message = "Có lỗi xảy ra khi cập nhật thông tin cá nhân",
                Success = false
            };
        }
        return DetailBaseResponse<string>.SuccessReturn("Thông tin cá nhân đã được cập nhật", string.Empty);
    }
}