using Application.Commands_Queries.Commands.Report;
using Application.Commons.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PRH_UserService_API.Extentions;

namespace PRH_UserService_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportController(ISender sender) : ControllerBase
{
    [HttpPost("send-report")]
    public async Task<IActionResult> SendReportAsync([FromBody] ReportMessageDto reportMessageDto)
    {
        reportMessageDto.context = HttpContext;
        var response = await sender.Send(new ReportCommand(reportMessageDto));
        return response.ToActionResult();
    }
}