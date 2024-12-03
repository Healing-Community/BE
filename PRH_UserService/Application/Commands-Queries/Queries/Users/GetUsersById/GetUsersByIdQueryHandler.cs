using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Queries.Queries.Users.GetUsersById;

public class GetUsersByIdQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUsersByIdQuery, BaseResponse<User>>
{
    public async Task<BaseResponse<User>> Handle(GetUsersByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(request.id);
            if (user == null)
            {
                return BaseResponse<User>.NotFound("User not found");
            }
            else
            {
                return BaseResponse<User>.SuccessReturn(user);
            }
        }
        catch (Exception e)
        {
            return BaseResponse<User>.InternalServerError(e.Message);
        }
    }
}