
using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;

namespace Application.Commands.Users.AddUser;

public class CreateUserCommandHandler(IUserRepository userRepository)
    : IRequestHandler<CreateUserCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userId = NewId.NextSequentialGuid();
        var user = new User
        {
            Id = userId,
            RoleId = request.UserDto.RoleId,
            Email = request.UserDto.Email,
            FullName = request.UserDto.FullName,
            Status = request.UserDto.Status,
            UserName = request.UserDto.UserName,
            PasswordHash = request.UserDto.PasswordHash,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var response = new BaseResponse<string>
        {
            Id = userId,
            Timestamp = DateTime.UtcNow
        };

        try
        {
            // Create user if name does not exist
            await userRepository.Create(user);
            response.Success = true;
            response.Errors = Enumerable.Empty<string>();
            response.Message = "User created successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors = new[] { ex.Message };
            response.Message = "Failed to create product";
        }

        return response;
    }
}

