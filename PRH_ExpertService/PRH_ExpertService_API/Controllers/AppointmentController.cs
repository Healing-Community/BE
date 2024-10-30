using Application.Commands.BookAppointment;
using Application.Commands.DeleteAppointment;
using Application.Commands.UpdateAppointment;
using Application.Queries.GetAllAppointments;
using Application.Queries.GetAppointments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_ExpertService_API.Extentions;

namespace PRH_ExpertService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController(ISender sender) : ControllerBase
    {
        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var response = await sender.Send(new GetAllAppointmentsQuery());
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpPost("book")]
        public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentCommand command)
        {
            var response = await sender.Send(command);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Expert")]
        [HttpGet("get/{expertProfileId}")]
        public async Task<IActionResult> GetAppointments(string expertProfileId)
        {
            var response = await sender.Send(new GetAppointmentsQuery(expertProfileId));
            return response.ToActionResult();
        }

        [Authorize(Roles = "Expert")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAppointment([FromBody] UpdateAppointmentCommand command)
        {
            var response = await sender.Send(command);
            return response.ToActionResult();
        }

        [Authorize(Roles = "User,Expert")]
        [HttpDelete("delete/{appointmentId}")]
        public async Task<IActionResult> DeleteAppointment([FromRoute] string appointmentId)
        {
            var response = await sender.Send(new DeleteAppointmentCommand(appointmentId));
            return response.ToActionResult();
        }
    }
}