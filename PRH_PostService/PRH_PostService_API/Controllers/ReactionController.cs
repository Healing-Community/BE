using Application.Commands.Reactions.AddReaction;
using Application.Commands.Reactions.DeleteReaction;
using Application.Commands.Reactions.UpdateReaction;
using Application.Commons.DTOs;
using Application.Queries.Reactions.GetPostReactionCount;
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
        /// <summary>
        /// Lấy số lượng reaction của bài viết
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("get-reaction-count/{postId}")]
        public async Task<IActionResult> GetPostReactionCount(string postId)
        {
            var postIdOnlyDto = new PostIdOnlyDto
            {
                PostId = postId
            };
            var response = await sender.Send(new GetPostReactionCountQuery(postIdOnlyDto));
            return response.ToActionResult();
        }
        /// <summary>
        /// Có thể không cần dùng
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Obsolete]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetReaction()
        {
            var response = await sender.Send(new GetReactionsQuery());
            return response.ToActionResult();
        }
        /// <summary>
        ///  Lấy reaction theo id (Có thể không cần dùng)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [Obsolete]
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await sender.Send(new GetReactionsByIdQuery(id));
            return response.ToActionResult();
        }
        /// <summary>
        /// Reaction bài viết id của reaction 1: Thích, 2: Haha, 3: Buồn, 4: Phãn nộ, 5: Yêu, 6: Wow
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
        /// <summary>
        /// Vì đang fix cứng 6 loai reaction nên có thể không cần dùng (dùng có thể gây bug)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Obsolete]
        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateReaction(string id, ReactionDto reaction)
        {
            var response = await sender.Send(new UpdateReactionCommand(id, reaction));
            return response.ToActionResult();
        }
        /// <summary>
        /// Vì đang fix cứng 6 loai reaction nên có thể không cần dùng (dùng có thể gây bug)
        /// </summary>
        /// <param name="removeReactionDto"></param>
        /// <returns></returns>
        [Obsolete]
        [Authorize]
        [HttpDelete("remove-reaction")]
        public async Task<IActionResult> DeleteReaction(PostIdOnlyDto removeReactionDto)
        {
            var response = await sender.Send(new DeleteReactionCommand(removeReactionDto));
            return response.ToActionResult();
        }
    }
}
