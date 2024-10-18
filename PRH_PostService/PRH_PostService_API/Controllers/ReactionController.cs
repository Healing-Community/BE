using Application.Commands.Posts.AddPost;
using Application.Commands.Posts.DeletePost;
using Application.Commands.Posts.UpdatePost;
using Application.Commands.Reactions.AddReaction;
using Application.Commands.Reactions.DeleteReaction;
using Application.Commands.Reactions.UpdateReaction;
using Application.Commons.DTOs;
using Application.Queries.Posts.GetPosts;
using Application.Queries.Posts.GetPostsById;
using Application.Queries.Reactions.GetReactions;
using Application.Queries.Reactions.GetReactionsById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRH_PostService_API.Extentions;

namespace PRH_PostService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReactionController(ISender sender) : ControllerBase
    {
        [Authorize(Roles = "User")]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetReaction()
        {
            var response = await sender.Send(new GetReactionsQuery());
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpGet("get-by-id/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await sender.Send(new GetReactionsByIdQuery(id));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateReaction(ReactionDto reaction)
        {
            var response = await sender.Send(new CreateReactionCommand(reaction, HttpContext));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpPut("update/{id:guid}")]
        public async Task<IActionResult> UpdateReaction(Guid id, ReactionDto reaction)
        {
            var response = await sender.Send(new UpdateReactionCommand(id, reaction));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpDelete("delete/{id:guid}")]
        public async Task<IActionResult> DeleteReaction(Guid id)
        {
            var response = await sender.Send(new DeleteReactionCommand(id));
            return response.ToActionResult();
        }
    }
}
