using Application.Commons;
using Application.Commons.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.ReportPosts.AddReport
{
    public record CreateReportCommand(ReportDto reportDto, HttpContext HttpContext) : IRequest<BaseResponse<string>>;
}
