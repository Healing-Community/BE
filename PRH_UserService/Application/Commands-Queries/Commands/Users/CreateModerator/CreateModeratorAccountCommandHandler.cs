using System;
using Application.Commons;
using Application.Commons.DTOs;
using Application.Commons.Enum;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;

namespace Application.Commands_Queries.Commands.Users.CreateModerator;

public class CreateModeratorAccountCommandHandler(IUserRepository userRepository) : IRequestHandler<CreateModeratorAccountCommand, DetailBaseResponse<string>>
{
    public async Task<DetailBaseResponse<string>> Handle(CreateModeratorAccountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var response = new DetailBaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<ErrorDetail>() // Khởi tạo danh sách lỗi theo định dạng yêu cầu
            };
            // Kiểm tra xem email đã được đăng ký hay chưa
            var existingUser = await userRepository.GetByPropertyAsync(u => u.Email == request.registerModeratorAccountDto.Email);
            if (existingUser != null)
            {
                // Nếu tài khoản tồn tại nhưng chưa được xác minh, xóa tài khoản đó
                if (existingUser.Status == 0)
                    await userRepository.DeleteAsync(existingUser.UserId);
                else
                    // Nếu tài khoản đã được xác minh, thông báo lỗi
                    response.Errors.Add(new ErrorDetail
                    {
                        Message = "Email đã được đăng ký.",
                        Field = "email"
                    });
            }
            // Kiểm tra xem tên người dùng đã tồn tại hay chưa
            var existingUserByUserName = await userRepository.GetUserByUserNameAsync(request.registerModeratorAccountDto.UserName);
            if (existingUserByUserName != null)
                response.Errors.Add(new ErrorDetail
                {
                    Message = "Tên người dùng đã bị sử dụng.",
                    Field = "username"
                });
            // Nếu có bất kỳ lỗi nào, trả về BadRequest và hiển thị các lỗi
            if (response.Errors.Count != 0)
            {
                response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                response.Success = false;
                response.Message = "Đã xảy ra lỗi trong quá trình đăng ký.";
                return response;
            }
            var user = new User
            {
                UserId = Ulid.NewUlid().ToString(),
                Email = request.registerModeratorAccountDto.Email,
                UserName = request.registerModeratorAccountDto.UserName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.registerModeratorAccountDto.Password),
                CreatedAt = DateTime.UtcNow + TimeSpan.FromHours(7),
                UpdatedAt = DateTime.UtcNow + TimeSpan.FromHours(7),
                Status = 0,
                // Mặc định là người dùng thường (role = 1), nếu đăng ký là chuyên gia thì role = 2
                RoleId = (int)EnumRole.Moderator
            };

            await userRepository.Create(user);
            response.Success = true;
            response.StatusCode = StatusCodes.Status201Created;
            response.Message = "Đăng ký tài khoản thành công.";
            return response;

        }
        catch (Exception e)
        {
            return new DetailBaseResponse<string>
            {
                Id = Ulid.NewUlid().ToString(),
                StatusCode = StatusCodes.Status500InternalServerError,
                Success = false,
                Message = e.Message,
                Errors = new List<ErrorDetail>()
            };
            throw;
        }
    }
}
