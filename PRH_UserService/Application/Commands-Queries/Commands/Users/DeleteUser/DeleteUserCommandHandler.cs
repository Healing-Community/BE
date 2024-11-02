using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.DeleteUser;

public class DeleteUserCommandHandler(IUserRepository userRepository)
    : IRequestHandler<DeleteUserCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<string>
        {
            Id = request.Id,
            Timestamp = DateTime.UtcNow,
            Errors = new List<string>() // Initialize the error list
        };

        try
        {
            await userRepository.DeleteAsync(request.Id);
            response.Success = true;
            response.Message = "User deleted successfully";
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = "Failed to delete user";
            response.Errors.Add(e.Message); // Add error message to the list
        }

        return response;
    }
}