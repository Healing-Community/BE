using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Queries.Groups.GetGroupsById
{
    public record GetGroupsByIdQuery(string Id) : IRequest<BaseResponse<Group>>;
}
