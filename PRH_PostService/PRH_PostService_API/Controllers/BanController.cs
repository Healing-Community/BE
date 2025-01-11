using Application.Commads_Queries.Commands.Comments.BanComment;
using Application.Commads_Queries.Commands.Posts.BanPost_Moderator;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_PostService_API.Extentions;

namespace PRH_PostService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BanController(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Moderator ban comment bằng cách ghi đè nội dung bình luận bằng chuỗi "Bình luận này đã bị ban do vi phạm quy định"
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Authorize(Roles = "Moderator")]
        [HttpPost("ban-comment")]
        public async Task<IActionResult> BanComment([FromBody] BanCommentCommand command)
        {
            var response = await sender.Send(command);
            return response.ToActionResult();
        }
        /// <summary>
        /// Moderator ban post bằng cách cập nhật trạng thái post thành "Banned"
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Authorize(Roles = "Moderator")]
        [HttpPost("ban-post")]
        public async Task<IActionResult> BanPost([FromBody] BanPostCommand command)
        {
            var response = await sender.Send(command);
            return response.ToActionResult();
        }
    }
}
