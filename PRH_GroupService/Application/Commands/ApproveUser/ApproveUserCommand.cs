using Application.Commons;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.ApproveUser
{
    public record ApproveUserCommand(string QueueId, bool IsApproved, HttpContext HttpContext) : IRequest<BaseResponse<string>>;
}
