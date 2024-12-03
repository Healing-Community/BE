using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Commands_Queries.Commands.Users.AddUser;

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
            CreatedAt = DateTime.UtcNow.AddHours(7), // Fixed UTC time to UTC+7
            UpdatedAt = DateTime.UtcNow.AddHours(7) // Fixed UTC time to UTC+7
        };
        try
        {
            // Create user if name does not exist
            await userRepository.Create(user);
            return BaseResponse<string>.SuccessReturn("User created successfully", userId);
        }
        catch (Exception ex)
        {
            return BaseResponse<string>.InternalServerError(ex.Message);
        }
    }
}