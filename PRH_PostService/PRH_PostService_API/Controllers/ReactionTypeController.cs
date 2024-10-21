using Application.Commands.ReactionTypes.AddReactionType;
using Application.Commands.ReactionTypes.DeleteReactionType;
using Application.Commands.ReactionTypes.UpdateReactionType;
using Application.Commons.DTOs;
using Application.Queries.ReactionTypes.GetReactionTypes;
using Application.Queries.ReactionTypes.GetReactionTypesById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_PostService_API.Extentions;

namespace PRH_PostService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReactionTypeController(ISender sender) : ControllerBase
    {
        [Authorize(Roles = "User")]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetReactionType()
        {
            var response = await sender.Send(new GetReactionTypesQuery());
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await sender.Send(new GetReactionTypesByIdQuery(id));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateReactionType(ReactionTypeDto reactionType)
        {
            var response = await sender.Send(new CreateReactionTypeCommand(reactionType));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateReactionType(string id, ReactionTypeDto reactionType)
        {
            var response = await sender.Send(new UpdateReactionTypeCommand(id, reactionType));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteReactionType(string id)
        {
            var response = await sender.Send(new DeleteReactionTypeCommand(id));
            return response.ToActionResult();
        }
    }
}
