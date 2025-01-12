using System;
using Application.Commands_Queries.Commnads.Report.Post.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PRH_ReportService_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController(ISender sender)  : ControllerBase
{
    [HttpPost("test")]
    public async Task<IActionResult> Test([FromBody] UpdatePostReportStatusCommand command)
    {
        var response = await sender.Send(command);
        return response.ToActionResult();
    }
}
