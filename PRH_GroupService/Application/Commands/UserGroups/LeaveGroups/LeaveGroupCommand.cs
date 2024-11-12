using Application.Commons;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.UserGroups.LeaveGroups
{
    public record LeaveGroupCommand(string GroupId, HttpContext HttpContext) : IRequest<BaseResponse<string>>;
}
