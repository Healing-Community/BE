using Application.Commons;
using Application.Commons.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.JoinGroups
{
    public record JoinGroupCommand(UserGroupDto UserGroupDto, HttpContext HttpContext) : IRequest<BaseResponse<string>>;
}
