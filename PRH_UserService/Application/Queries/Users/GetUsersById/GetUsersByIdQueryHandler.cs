using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Users.GetUsersById
{
    public class GetUsersByIdQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUsersByIdQuery, BaseResponse<User>>
    {
        public async Task<BaseResponse<User>> Handle(GetUsersByIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<User>()
            {
                Id = NewId.NextSequentialGuid(),
                Timestamp = DateTime.UtcNow,
            };
            try
            {
                var user = await userRepository.GetByIdAsync(request.id);
                response.Data = user;
                response.Success = true;
                response.Message = "User retrieved successfully";
                response.Errors = Enumerable.Empty<string>();
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = "An error occurred";
                response.Errors = new[] { e.Message };
            }
            return response;
        }
    }
}
