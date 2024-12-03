using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Commands_Queries.Queries.Roles.GetRoles;

public class GetRolesQueryHandler(IRoleRepository roleRepository)
    : IRequestHandler<GetRolesQuery, BaseResponse<IEnumerable<Role>>>
{
    public async Task<BaseResponse<IEnumerable<Role>>> Handle(GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {            var roles = await roleRepository.GetsAsync();
            if (roles == null || !roles.Any())
            {
                return BaseResponse<IEnumerable<Role>>.NotFound("Không tìm thấy Role.");
            }
            else
            {
                return BaseResponse<IEnumerable<Role>>.SuccessReturn(roles);
            }
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<Role>>.InternalServerError(e.Message);
        }
    }
}