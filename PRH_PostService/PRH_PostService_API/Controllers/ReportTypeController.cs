using Application.Commands.ReportPostTypes.AddReportType;
using Application.Commons.DTOs;
using Application.Queries.ReportPostTypes.GetReportTypes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_PostService_API.Extentions;

namespace PRH_PostService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportTypeController(ISender sender) : ControllerBase
    {

        [Authorize]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetReportType()
        {
            var response = await sender.Send(new GetReportTypesQuery());
            return response.ToActionResult();
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateReportType(ReportTypeDto reportType)
        {
            var response = await sender.Send(new CreateReportTypeCommand(reportType, HttpContext));
            return response.ToActionResult();
        }
    }
}
