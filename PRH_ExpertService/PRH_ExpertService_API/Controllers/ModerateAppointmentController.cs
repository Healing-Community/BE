using Application.Commands_Quereis.Commands.ModerateAppointmentReport;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRH_ExpertService_API.Extentions;

namespace PRH_ExpertService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModerateAppointmentController(ISender sender) : ControllerBase
    {
        [Authorize(Roles = "Moderator")]
        [HttpPost("moderate-appointment-report")]
        public async Task<IActionResult> ModerateAppointmentReport(ModerateAppointmentReportCommand command)
        {
            var response = await sender.Send(command);
            return response.ToActionResult();
        }
    }
}
