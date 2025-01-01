using Application.Commads_Queries.Commands.CreateShare;
using Application.Commads_Queries.Commands.Share.DeleteShare;
using Application.Commads_Queries.Commands.Share.UpdateShare;
using Application.Commads_Queries.Queries.Share;
using Application.Commads_Queries.Queries.Share.CountShareByPostId;
using Application.Commons.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRH_PostService_API.Extentions;

namespace PRH_PostService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShareController(ISender sender) : ControllerBase
    {
        /// <summary>
        ///  Đếm tổng lượt chia sẻ của bài Post
        /// </summary>
        /// /// <param name="postId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("count-share/{postId}")]
        public async Task<IActionResult> CountShareByPostId(string postId)
        {
            var postIdOnlyDto = new PostIdOnlyDto
            {
                PostId = postId
            };
            var response = await sender.Send(new CountShareByPostIdQuery(postIdOnlyDto));
            return response.ToActionResult();
        }
        /// <summary>
        ///  Lấy danh sách các bài viết đã chia sẻ chỉ lấy các baì nội bộ
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("get-share-posts/{userId}")]
        public async Task<IActionResult> GetSharePosts(string userId)
        {
            var response = await sender.Send(new GetSharePostQuery(userId));
            return response.ToActionResult();
        }
        /// <summary>
        ///  Lưu ý: Platform mặc định là "Internal" nên chỉ cần truyền PostId còn muốn chia sẻ qua mạng xã hội thì truyền Platform = "Facebook" hoặc "Email",..etc
        /// </summary>
        /// <param name="shareDto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("share-story")]
        public async Task<IActionResult> ShareStory([FromBody] ShareDto shareDto)
        {
            var response = await sender.Send(new CreateShareCommand(shareDto));
            return response.ToActionResult();
        }
        /// <summary>
        /// Cập nhật bài viết đã chia sẻ
        /// </summary>
        /// <param name="editShareDto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("update-share")]
        public async Task<IActionResult> UpdateShare([FromBody] EditShareDto editShareDto)
        {
            var response = await sender.Send(new UpdateShareCommand(editShareDto));
            return response.ToActionResult();
        }
        /// <summary>
        /// Xóa bài viết đã chia sẻ
        /// </summary>
        /// <param name="shareId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("delete-share/{shareId}")]
        public async Task<IActionResult> DeleteShare(string shareId)
        {
            var response = await sender.Send(new DeleteShareCommand(shareId));
            return response.ToActionResult();
        }
    }
}
