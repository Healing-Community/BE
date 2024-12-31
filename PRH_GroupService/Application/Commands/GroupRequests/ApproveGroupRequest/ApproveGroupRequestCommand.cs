using Application.Commons;
using MediatR;

namespace Application.Commands.GroupRequests.ApproveGroupRequest
{
    public record ApproveGroupRequestCommand(string GroupRequestId, bool IsApproved, string ApprovedById) : IRequest<BaseResponse<string>>;
}
