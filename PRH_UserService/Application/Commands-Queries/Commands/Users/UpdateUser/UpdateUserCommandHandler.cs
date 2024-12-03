using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Commands_Queries.Commands.Users.UpdateUser;

public class UpdateUserCommandHandler(IUserRepository userRepository)
    : IRequestHandler<UpdateUserCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var existingUser = await userRepository.GetByIdAsync(request.Id);
            if (existingUser == null)
            {
                return BaseResponse<string>.NotFound("không tìm thấy người dùng");
            }

            var updatedUser = new User
            {
                UserId = request.Id,
                Email = request.UserDto.Email,
                FullName = request.UserDto.FullName,
                Status = request.UserDto.Status,
                UserName = request.UserDto.UserName,
                PasswordHash = request.UserDto.PasswordHash,
                CreatedAt = existingUser.CreatedAt, // Keep original creation date
                UpdatedAt = DateTime.UtcNow,
                RoleId = request.UserDto.RoleId
            };

            await userRepository.UpdateAsync(request.Id, updatedUser);
            
            return BaseResponse<string>.SuccessReturn("Cập nhật người dùng thành công");
        }
        catch (Exception ex)
        {
            return BaseResponse<string>.InternalServerError(ex.Message);
        }
    }
}