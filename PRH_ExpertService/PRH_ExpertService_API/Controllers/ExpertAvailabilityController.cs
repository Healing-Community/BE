using Application.Commands.CreateAvailability;
using Application.Commands.UpdateAvailability;
using Application.Commands.DeleteAvailability;
using Application.Queries.GetAvailability;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_ExpertService_API.Extentions;

namespace PRH_ExpertService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpertAvailabilityController(ISender sender) : ControllerBase
    {
        [Authorize(Roles = "Expert")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateAvailability([FromBody] CreateAvailabilityCommand command)
        {
            var response = await sender.Send(command);
            return response.ToActionResult();
        }

        [Authorize]
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
        [HttpDelete("delete/{availabilityId}")]
        public async Task<IActionResult> DeleteAvailability(string availabilityId)
        {
            var response = await sender.Send(new DeleteAvailabilityCommand(availabilityId));
            return response.ToActionResult();
        }
    }
}