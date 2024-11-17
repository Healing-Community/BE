using Application.Commons;
using Domain.Entities;
using MediatR;
namespace Application.Queries.UserGroups.GetUserGroups
{
    public record GetUserGroupsQuery : IRequest<BaseResponse<IEnumerable<UserGroup>>>;
}
