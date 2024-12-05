using Application.Commands.CreateWorkExperience;
using Application.Commands.DeleteWorkExperience;
using Application.Commands.UpdateWorkExperience;
using Application.Queries.GetAllWorkExperiences;
using Application.Queries.GetWorkExperienceQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_ExpertService_API.Extentions;

namespace PRH_ExpertService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkExperienceController(ISender sender) : ControllerBase
    {
        [Authorize(Roles = "Admin,Expert")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllWorkExperiences()
        {
            var response = await sender.Send(new GetAllWorkExperiencesQuery());
            return response.ToActionResult();
        }

        [Authorize(Roles = "Expert")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateWorkExperience([FromBody] CreateWorkExperienceCommand command)
        {
            var response = await sender.Send(command);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Expert")]
        [HttpGet("get")]
        public async Task<IActionResult> GetWorkExperience()
        {
            var response = await sender.Send(new GetWorkExperienceQuery());
            return response.ToActionResult();
        }

        [Authorize(Roles = "Expert")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateWorkExperience([FromBody] UpdateWorkExperienceCommand command)
        {
            var response = await sender.Send(command);
            return response.ToActionResult();
        }

        [Authorize(Roles = "Expert")]
        [HttpDelete("delete/{workExperienceId}")]
        public async Task<IActionResult> DeleteWorkExperience(string workExperienceId)
        {
            var response = await sender.Send(new DeleteWorkExperienceCommand(workExperienceId));
            return response.ToActionResult();
        }
    }
}
