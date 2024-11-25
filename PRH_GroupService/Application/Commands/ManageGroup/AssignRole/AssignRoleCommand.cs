using Application.Commons;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.ManageGroup.AssignRole
{
    public record AssignRoleCommand(string GroupId, string UserId, string Role, HttpContext HttpContext) : IRequest<BaseResponse<string>>;
}
