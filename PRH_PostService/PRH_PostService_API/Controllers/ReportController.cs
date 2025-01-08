using Application.Commads_Queries.Commands.Comments.ReportComment;
using Application.Commands.ReportPosts.AddReport;
using Application.Commons.DTOs;
using Application.Queries.ReportPosts.GetReports;
using Domain.Entities;
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
        // [Obsolete]
        // [Authorize]
        // [HttpGet("get-all")]
        // public async Task<IActionResult> GetReport()
        // {
        //     var response = await sender.Send(new GetReportsQuery());
        //     return response.ToActionResult();
        // }

        [Authorize]
        [HttpPost("report-post")]
        public async Task<IActionResult> CreateReport(ReportDto report)
        {
            var response = await sender.Send(new CreateReportCommand(report));
            return response.ToActionResult();
        }
        [Authorize]
        [HttpPost("report-comment")]
        public async Task<IActionResult> CreateReport(ReportCommentDto report)
        {
            var response = await sender.Send(new ReportCommentCommand(report));
            return response.ToActionResult();
        }
    }
}
