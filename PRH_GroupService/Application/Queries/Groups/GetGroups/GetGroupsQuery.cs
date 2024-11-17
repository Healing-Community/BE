using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Groups.GetGroups
{
    public record GetGroupsQuery : IRequest<BaseResponse<IEnumerable<Group>>>;
}
