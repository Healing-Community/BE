using Application.Commands.Reactions.AddReaction;
using Application.Commands.Reactions.DeleteReaction;
using Application.Commands.Reactions.UpdateReaction;
using Application.Commons.DTOs;
using Application.Queries.Reactions.GetReactions;
using Application.Queries.Reactions.GetReactionsById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_PostService_API.Extentions;

namespace PRH_PostService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReactionController(ISender sender) : ControllerBase
    {
        [Authorize]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetReaction()
        {
            var response = await sender.Send(new GetReactionsQuery());
            return response.ToActionResult();
        }

        [Authorize]
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await sender.Send(new GetReactionsByIdQuery(id));
            return response.ToActionResult();
        }
        /// <summary>
        /// 1: Thích, 2: Haha, 3: Buồn, 4: Phãn nộ, 5: Yêu, 6: Wow
        /// </summary>
        /// <param name="reaction"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("add-reaction")]
        public async Task<IActionResult> CreateReaction(ReactionDto reaction)
        {
            var response = await sender.Send(new CreateReactionCommand(reaction, HttpContext));
            return response.ToActionResult();
        }

        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateReaction(string id, ReactionDto reaction)
        {
            var response = await sender.Send(new UpdateReactionCommand(id, reaction));
            return response.ToActionResult();
        }
        [Authorize]
        [HttpDelete("remove-reaction")]
        public async Task<IActionResult> DeleteReaction(RemoveReactionDto removeReactionDto)
        {
            var response = await sender.Send(new DeleteReactionCommand(removeReactionDto));
            return response.ToActionResult();
        }
    }
}
