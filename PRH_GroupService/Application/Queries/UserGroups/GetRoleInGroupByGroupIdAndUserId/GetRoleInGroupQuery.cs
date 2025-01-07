using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Queries.UserGroups.GetRoleInGroupByGroupIdAndUserId
{
    public record GetRoleInGroupQuery(string UserId, string GroupId) : IRequest<BaseResponse<RoleInGroupDto>>;
}
