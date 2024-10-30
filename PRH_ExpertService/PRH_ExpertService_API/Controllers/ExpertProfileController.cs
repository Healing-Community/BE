using Application.Queries.GetAllExpertProfiles;
using Application.Queries.GetExpertProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_ExpertService_API.Extentions;

namespace PRH_ExpertService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpertProfileController(ISender sender) : ControllerBase
    {
        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllExpertProfiles()
        {
            var response = await sender.Send(new GetAllExpertProfilesQuery());
            return response.ToActionResult();
        }

        [Authorize]
        [HttpGet("profile/{expertId}")]
        public async Task<IActionResult> GetExpertProfile([FromRoute] string expertId)
        {
            var response = await sender.Send(new GetExpertProfileQuery(expertId));
            return response.ToActionResult();
        }
    }
}
