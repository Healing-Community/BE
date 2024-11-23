using Application.Commons.DTOs;
using Application.Commons;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Queries.Groups.GetApprovalQueue
{
    public record GetAllApprovalQueueQuery(string GroupId, HttpContext HttpContext) : IRequest<BaseResponse<IEnumerable<ApprovalQueueDto>>>;
}
