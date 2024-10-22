using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Queries.Users.GetUsers;

public class GetUsersQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUsersQuery, BaseResponse<IEnumerable<User>>>
{
    public async Task<BaseResponse<IEnumerable<User>>> Handle(GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var response = new BaseResponse<IEnumerable<User>>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.UtcNow
        };
        try
        {
            var users = await userRepository.GetsAsync();
            response.Message = "Users retrieved successfully";
            response.Success = true;
            response.Data = (IEnumerable<User>?)users;
        }
        catch (Exception e)
        {
            response.Message = e.Message;
            response.Success = false;
        }

        return response;
    }
}