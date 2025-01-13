using Application.Queries.ModeratorActivity.AppointmentReportActivity;
using Application.Queries.ModeratorActivity.CommentReportActivity;
using Application.Queries.ModeratorActivity.PostReportActivity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PRH_ReportService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModeratorActivityController(ISender sender) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("get-comment-report-activity")]
        public async Task<IActionResult> GetCommentReportActivity()
        {
            var response = await sender.Send(new GetCommentReportActivityQuery());
            return response.ToActionResult();
        }

        [Authorize (Roles = "Admin")]
        [HttpGet("get-post-report-activity")]
        public async Task<IActionResult> GetPostReportActivity()
        {
            var response = await sender.Send(new GetPostReportActivityQuery());
            return response.ToActionResult();
        }

    }
}
