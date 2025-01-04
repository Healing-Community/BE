using Application.Commands_Queries.Commands.PlatformFees.UpdateFee;
using Application.Commands_Queries.Queries.GetPlatformFee;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PRH_PaymentService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformFeeController(ISender sender) : ControllerBase
    {
        [HttpGet("get-fees")]
        public async Task<IActionResult> GetPlatformFee()
        {
            var response = await sender.Send(new GetPlatformFeeQuery());
            return response.ToActionResult();
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("update-fee")]
        public async Task<IActionResult> UpdatePlatformFee([FromBody] UpdateFeeCommand command)
        {
            var response = await sender.Send(command);
            return response.ToActionResult();
        }
    }
}
