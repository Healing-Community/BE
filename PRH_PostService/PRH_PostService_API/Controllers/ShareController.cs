using Application.Commads_Queries.Commands.CreateShare;
using Application.Commads_Queries.Queries.Share;
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
    }
}
