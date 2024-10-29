using Application.Commons;
using Application.Commons.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Groups.AddGroup
{
    public record CreateGroupCommand(GroupDto groupDto, HttpContext httpContext) : IRequest<BaseResponse<string>>
    {
    }
}
