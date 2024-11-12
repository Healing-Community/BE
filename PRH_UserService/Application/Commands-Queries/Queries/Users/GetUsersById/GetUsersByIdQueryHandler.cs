using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Commands_Queries.Queries.Users.GetUsersById;

public class GetUsersByIdQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUsersByIdQuery, BaseResponse<User>>
{
    public async Task<BaseResponse<User>> Handle(GetUsersByIdQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<User>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.UtcNow,
            Errors = new List<string>() // Initialize the error list
        };

        try
        {
            var user = await userRepository.GetByIdAsync(request.id);
            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found";
                response.Errors.Add("No user found with the provided ID.");
                return response;
            }

            response.Data = user;
            response.Success = true;
            response.Message = "User retrieved successfully";
        }
        catch (Exception e)
        {
            response.Success = false;
            response.Message = "An error occurred while retrieving the user";
            response.Errors.Add(e.Message); // Add error message to the list
        }

        return response;
    }
}