using Application.Commons;
using Application.Commons.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
namespace Application.Commands.ReportPostTypes.AddReportType
{
    public record CreateReportTypeCommand(ReportTypeDto ReportTypeDto, HttpContext HttpContext) : IRequest<BaseResponse<string>>;
}
