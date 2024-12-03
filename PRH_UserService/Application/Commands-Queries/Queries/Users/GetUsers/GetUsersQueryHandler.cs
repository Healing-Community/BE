using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Queries.Queries.Users.GetUsers;

public class GetUsersQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUsersQuery, BaseResponse<IEnumerable<User>>>
{
    public async Task<BaseResponse<IEnumerable<User>>> Handle(GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var users = await userRepository.GetsAsync();
            if (users == null || !users.Any())
            {
                return BaseResponse<IEnumerable<User>>.NotFound("Không tìm thấy người dùng.");
            }
            else
            {
                return BaseResponse<IEnumerable<User>>.SuccessReturn(users);
            }
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<User>>.InternalServerError(e.Message);
        }
    }
}