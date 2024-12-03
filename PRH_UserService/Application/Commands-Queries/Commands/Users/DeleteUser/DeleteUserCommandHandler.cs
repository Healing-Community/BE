using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commands_Queries.Commands.Users.DeleteUser;

public class DeleteUserCommandHandler(IUserRepository userRepository)
    : IRequestHandler<DeleteUserCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await userRepository.DeleteAsync(request.Id);
            return BaseResponse<string>.SuccessReturn("User deleted successfully");
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
        }
    }
}