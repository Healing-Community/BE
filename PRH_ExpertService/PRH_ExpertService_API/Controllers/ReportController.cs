using Application.Commands_Quereis.Commands.ReportAppoinmet;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_ExpertService_API.Extentions;

namespace PRH_ExpertService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController(ISender sender) : ControllerBase
    {
        [Authorize(Roles = "User")]
        [HttpPost("report-appointment")]
        public async Task<IActionResult> ReportAppointment([FromBody] ReportAppointmentCommand command)
        {
            var response = await sender.Send(command);
            return response.ToActionResult();
        }
    }
}
