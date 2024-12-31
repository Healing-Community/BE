using Application.Commons.DTOs;
using Application.Commons;
using MediatR;

namespace Application.Queries.GroupRequests.GetUserRequests
{
    public record GetUserRequestsQuery(string UserId) : IRequest<BaseResponse<IEnumerable<GroupRequestDto>>>;
}
