using Application.Commons;
using MediatR;

namespace Application.Commands.GroupRequests.CreateGroupRequest
{
    public record CreateGroupRequestCommand(string GroupName, string Description) : IRequest<BaseResponse<string>>;
}
