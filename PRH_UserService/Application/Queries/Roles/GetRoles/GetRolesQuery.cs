using Application.Commons;
using Domain.Entities;
using MediatR;


namespace Application.Queries.Roles.GetRoles
{
    public record GetRolesQuery : IRequest<BaseResponse<IEnumerable<Role>>>;
}
