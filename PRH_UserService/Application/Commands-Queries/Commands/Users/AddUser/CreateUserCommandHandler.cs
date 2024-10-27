using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Commands.Users.AddUser;

public class CreateUserCommandHandler(IUserRepository userRepository)
    : IRequestHandler<CreateUserCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userId = Ulid.NewUlid().ToString();
        var user = new User
        {
            UserId = userId,
            RoleId = request.UserDto.RoleId,
            Email = request.UserDto.Email,
            FullName = request.UserDto.FullName,
            Status = request.UserDto.Status,
            UserName = request.UserDto.UserName,
            PasswordHash = request.UserDto.PasswordHash,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var response = new BaseResponse<string>
        {
            Id = userId,
            Timestamp = DateTime.UtcNow,
            Errors = new List<string>() // Initialize the error list
        };

        try
        {
            // Create user if name does not exist
            await userRepository.Create(user);
            response.Success = true;
            response.Message = "User created successfully";
            response.StatusCode = 200;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add(ex.Message); // Add error message to the list
            response.Message = "Failed to create user"; // Fixed typo from "product" to "user"
        }

        return response;
    }
}
