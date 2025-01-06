using Application.Commons;
using MediatR;

namespace Application.Commands.GroupRequests.CreateGroupRequest
{
    public record CreateGroupRequestCommand(string GroupName, string Description , string CoverImg) : IRequest<BaseResponse<string>>;
}
