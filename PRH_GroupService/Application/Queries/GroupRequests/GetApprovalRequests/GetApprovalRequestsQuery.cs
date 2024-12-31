using Application.Commons.DTOs;
using Application.Commons;
using MediatR;

namespace Application.Queries.GroupRequests.GetApprovalRequests
{
    public record GetApprovalRequestsQuery(string UserRole) : IRequest<BaseResponse<IEnumerable<GroupRequestDto>>>;
}
