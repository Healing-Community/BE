using Application.Commads_Queries.Commands.Reactions.AddReactionForShare;
using Application.Commads_Queries.Commands.Reactions.DeleteReactionByShareId;
using Application.Commads_Queries.Queries.Reactions.GetShareReactionCount;
using Application.Commads_Queries.Queries.Reactions.GetUserReactionByShareId;
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
        /// Lấy reaction của user theo post id
        /// </summary>
        /// <param name="PostId"></param>
        /// <returns></returns>
        [HttpGet("get-user-reaction-by-post-id/{PostId}")]
        public async Task<IActionResult> GetUserReactionByPostId(string PostId)
        {
            var response = await sender.Send(new GetUserReactionByPostIdQuery(PostId));
            return response.ToActionResult();
        }
        /// <summary>
        /// Lấy reaction của user theo share id
        /// </summary>
        /// <param name="ShareId"></param>
        /// <returns></returns>
        [HttpGet("get-user-reaction-by-share-id/{ShareId}")]
        public async Task<IActionResult> GetUserReactionByShareId(string ShareId)
        {
            var response = await sender.Send(new GetUserReactionByShareIdQuery(ShareId));
            return response.ToActionResult();
        }
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
        /// Lấy số lượng reaction của bài viết được chia sẻ
        /// </summary>
        /// <param name="shareId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("get-reaction-count-by-share/{shareId}")]
        public async Task<IActionResult> GetShareReactionCount(string shareId)
        {
            var shareIdOnlyDto = new ShareIdOnlyDto
            {
                ShareId = shareId
            };
            var response = await sender.Send(new GetShareReactionCountQuery(shareIdOnlyDto));
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
        /// Reaction bài viết được chia sẻ id của reaction 1: Thích, 2: Haha, 3: Buồn, 4: Phãn nộ, 5: Yêu, 6: Wow
        /// </summary>
        /// <param name="reaction"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("add-reaction-share")]
        public async Task<IActionResult> CreateReactionForShare(ReactionShareDto reaction)
        {
            var response = await sender.Send(new CreateReactionForShareCommand(reaction, HttpContext));
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
        [Authorize]
        [HttpDelete("remove-reaction")]
        public async Task<IActionResult> DeleteReaction(PostIdOnlyDto removeReactionDto)
        {
            var response = await sender.Send(new DeleteReactionCommand(removeReactionDto));
            return response.ToActionResult();
        }
        [Authorize]
        [HttpDelete("remove-reaction-by-share")]
        public async Task<IActionResult> DeleteReactionByShareId(ShareIdOnlyDto removeReactionDto)
        {
            var response = await sender.Send(new DeleteReactionByShareIdCommand(removeReactionDto));
            return response.ToActionResult();
        }
    }
}
