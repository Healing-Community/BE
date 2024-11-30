using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Queries.UserGroups.GetUserGroupsByGroupId
{
    public record GetUserGroupsByGroupIdQuery(string GroupId) : IRequest<BaseResponse<List<UserGroupByGroupIdDto>>>;
}

