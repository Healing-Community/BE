using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Queries.UserGroups.GetUserGroupsByUserId
{
    public record GetUserGroupsByUserIdQuery(string UserId) : IRequest<BaseResponse<List<UserGroupByUserIdDto>>>;
}
