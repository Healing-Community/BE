using Application.Commons;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Groups.RemoveMember
{
    public record RemoveMemberCommand(string GroupId, string MemberUserId, HttpContext HttpContext) : IRequest<BaseResponse<string>>;
}
