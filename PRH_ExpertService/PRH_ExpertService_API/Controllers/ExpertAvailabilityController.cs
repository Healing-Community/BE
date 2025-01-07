using Application.Commands.CreateAvailability;
using Application.Commands.UpdateAvailability;
using Application.Commands.DeleteAvailability;
using Application.Queries.GetAvailability;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_ExpertService_API.Extentions;
using Application.Queries.GetAllExpertAvailabilities;
using Domain.Entities;
using Application.Queries.GetExpertAvailbilityByAppointmentId;

namespace PRH_ExpertService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpertAvailabilityController(ISender sender) : ControllerBase
    {
        [HttpGet("get-by-appointmentId/{appointmentId}")]
        public async Task<IActionResult> GetExpertAvailabilityByAppointmentId(string appointmentId)
        {
            var response = await sender.Send(new GetExpertAvailbilityByAppointmentIdQuery(appointmentId));
            return response.ToActionResult();
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllExpertAvailabilities()
        {
            var response = await sender.Send(new GetAllExpertAvailabilitiesQuery());
            return response.ToActionResult();
        }

        [Authorize(Roles = "Expert")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateAvailability([FromBody] CreateAvailabilityCommand command)
        {
            var response = await sender.Send(command);
            return response.ToActionResult();
        }

        [Authorize(Roles = "User,Expert")]
        [HttpGet("get/{expertProfileId}")]
        public async Task<IActionResult> GetAvailability(string expertProfileId)
        {
            var response = await sender.Send(new GetAvailabilityQuery(expertProfileId));
            return response.ToActionResult();
        }

        [Authorize(Roles = "Expert")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAvailability([FromBody] UpdateAvailabilityCommand command)
        {
            var response = await sender.Send(command);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Expert")]
        [HttpDelete("delete/{expertAvailabilityId}")]
        public async Task<IActionResult> DeleteAvailability(string expertAvailabilityId)
        {
            var response = await sender.Send(new DeleteAvailabilityCommand(expertAvailabilityId));
            return response.ToActionResult();
        }
    }
}
