using Application.Commands.UpdateExpertProfile;
using Application.Commands.DeleteExpertProfile;
using Application.Queries.GetAllExpertProfiles;
using Application.Queries.GetExpertProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_ExpertService_API.Extentions;
using Application.Commands.UploadProfileImage;
using PRH_ExpertService_API.FileUpload;
using Application.Queries.GetExpertList;

namespace PRH_ExpertService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpertProfileController(ISender sender) : ControllerBase
    {
        [Authorize(Roles = "Admin,Expert")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllExpertProfiles()
        {
            var response = await sender.Send(new GetAllExpertProfilesQuery());
            return response.ToActionResult();
        }

        //[Authorize(Roles = "Expert")]
        //[HttpPost("create")]
        //public async Task<IActionResult> CreateExpertProfile([FromBody] CreateExpertProfileCommand command)
        //{
        //    var response = await sender.Send(command);
        //    return response.ToActionResult();
        //}

        [Authorize]
        [HttpGet("profile/{expertProfileId}")]
        public async Task<IActionResult> GetExpertProfile([FromRoute] string expertProfileId)
        {
            var response = await sender.Send(new GetExpertProfileQuery(expertProfileId));
            return response.ToActionResult();
        }

        [Authorize(Roles = "Expert")]
        [HttpPost("upload-profile-image")]
        public async Task<IActionResult> UploadProfileImage([FromForm] FileUploadModel model)
        {
            var response = await sender.Send(new UploadProfileImageCommand(model.File));
            return response.ToActionResult();
        }

        [Authorize(Roles = "Expert")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateExpertProfile([FromBody] UpdateExpertProfileCommand command)
        {
            var response = await sender.Send(command);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Admin,Expert")]
        [HttpDelete("delete/{expertId}")]
        public async Task<IActionResult> DeleteExpertProfile([FromRoute] string expertId)
        {
            var command = new DeleteExpertProfileCommand(expertId);
            var response = await sender.Send(command);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Expert,User")]
        [HttpGet("expert-list")]
        public async Task<IActionResult> GetExpertList([FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var response = await sender.Send(new GetExpertListQuery(pageNumber, pageSize));
            return response.ToActionResult();
        }

    }
}
