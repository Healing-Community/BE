using Application.Commons.DTOs;
using Application.Commons;
using MediatR;

namespace Application.Queries.UserGroups.GetRoleInGroup
{
    public record GetRoleInGroupByGroupIdQuery(string GroupId) : IRequest<BaseResponse<RoleCountDto>>;
}
