using Application.Commands.ReportPosts.AddReport;
using Application.Commons.DTOs;
using Application.Queries.ReportPosts.GetReports;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_PostService_API.Extentions;

namespace PRH_PostService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController(ISender sender) : ControllerBase
    {

        [Authorize(Roles = "User")]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetReport()
        {
            var response = await sender.Send(new GetReportsQuery());
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateReport(ReportDto report)
        {
            var response = await sender.Send(new CreateReportCommand(report, HttpContext));
            return response.ToActionResult();
        }
    }
}
