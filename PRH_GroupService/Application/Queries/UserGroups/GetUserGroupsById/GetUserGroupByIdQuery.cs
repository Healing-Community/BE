using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.UserGroups.GetUserGroupsById
{
    public record GetUserGroupByIdQuery(string GroupId, string UserId) : IRequest<BaseResponse<UserGroup>>;
}
