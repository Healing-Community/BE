using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Users.GetUsers
{
    public record GetUsersQuery : IRequest<BaseResponse<IEnumerable<User>>>;
}
