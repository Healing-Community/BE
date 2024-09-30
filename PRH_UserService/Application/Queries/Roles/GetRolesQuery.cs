using Application.Commons;
using Domain.Entities;
using MediatR;


namespace Application.Queries.Roles
{
    public record GetRolesQuery : IRequest<BaseResponse<IEnumerable<Role>>>;
}
