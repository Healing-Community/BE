using Application.Commads_Queries.Commands.Bookmarks.AddBookMark;
using Application.Commads_Queries.Commands.Bookmarks.AddBookmarkPost;
using Application.Commads_Queries.Commands.Bookmarks.DeleteBookmark;
using Application.Commads_Queries.Commands.Bookmarks.DeletePostFromBookmark;
using Application.Commads_Queries.Queries.Bookmarks.GetsPostBookmark;
using Application.Commons.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_PostService_API.Extentions;

namespace PRH_PostService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookmarkController(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Lấy danh sách bookmark của user để load lên combo box khi tạo bookmark post
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("get-bookmark")]
        public async Task<IActionResult> GetBookmark()
        {
            var response = await sender.Send(new GetBookmarkQuery());
            return response.ToActionResult();
        }
        /// <summary>
        ///  lấy danh sách bài viết đã bookmark của user, user chỉ có thể xem bookmark của chính mình
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("get-post-bookmark")]
        public async Task<IActionResult> GetPostBookmark(string bookmarkId)
        {
            var response = await sender.Send(new GetsPostBookmark(bookmarkId));
            return response.ToActionResult();
        }
        /// <summary>
        /// tạo bookmark: mỗi user có thể tạo nhiều bookmark và bookmark chỉ thuộc về user tạo ra nó
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("create-bookmark")]
        public async Task<IActionResult> CreateBookmark([FromBody]string name)
        {
            var response = await sender.Send(new AddBookmarkCommand(name));
            return response.ToActionResult();
        }

        /// <summary>
        ///  thêm bài viết vào bookmark đã tạo
        /// </summary>
        /// <param name="bookmarkPostDto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("add-bookmark-post")]
        public async Task<IActionResult> AddBookmarkPost(BookmarkPostDto bookmarkPostDto)
        {
            var response = await sender.Send(new AddBookmarkPostCommand(bookmarkPostDto));
            return response.ToActionResult();
        }
        /// <summary>
        /// xóa bài viết khỏi bookmark
        /// </summary>
        /// <param name="bookmarkPostDto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("delete-bookmark-post")]
        public async Task<IActionResult> DeletePostFromBookmark(BookmarkPostDto bookmarkPostDto)
        {
            var response = await sender.Send(new DeletePostFromBookmarkCommand(bookmarkPostDto));
            return response.ToActionResult();
        }
        /// <summary>
        /// xóa bookmark
        /// </summary>
        /// <param name="bookmarkId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("delete-bookmark")]
        public async Task<IActionResult> DeleteBookmark([FromBody]string bookmarkId)
        {
            var response = await sender.Send(new DeleteBookmarkCommand(bookmarkId));
            return response.ToActionResult();
        }
    }
}
