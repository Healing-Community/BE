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
        var response = new BaseResponse<string>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.UtcNow,
            Errors = new List<string>() // Initialize the error list
        };

        try
        {
            var existingUser = await userRepository.GetByIdAsync(request.Id);
            if (existingUser == null)
            {
                response.Success = false;
                response.Message = "User not found";
                response.Errors.Add("No user found with the provided ID.");
                return response;
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
            response.Success = true;
            response.Message = "User updated successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Failed to update user";
            response.Errors.Add(ex.Message); // Add error message to the list
        }

        return response;
    }
}