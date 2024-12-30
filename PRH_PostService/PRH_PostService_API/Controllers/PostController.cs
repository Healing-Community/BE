using Application.Commads_Queries.Commands.Posts.UpdatePostInGroup;
using Application.Commads_Queries.Queries.Posts.GetOtherPostByAutour;
using Application.Commads_Queries.Queries.Posts.GetPostsInGroupByGroupId;
using Application.Commads_Queries.Queries.Posts.GetPostsInGroupByUserAndGroup;
using Application.Commads_Queries.Queries.Posts.GetPostsInGroups;
using Application.Commads_Queries.Queries.Posts.GetRelativeCatogoryPost;
using Application.Commands.Posts.AddPost;
using Application.Commands.Posts.AddPostGroup;
using Application.Commands.Posts.DeletePost;
using Application.Commands.Posts.UpdatePost;
using Application.Commands.UserReference;
using Application.CommandsQueries.Queries.Posts.GetsTopPost;
using Application.Commons.DTOs;
using Application.Queries.Posts.GetPosts;
using Application.Queries.Posts.GetPostsById;
using Application.Queries.Posts.GetPostsByUserId;
using Application.Queries.Posts.GetPrivatePost;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_PostService_API.Extentions;

namespace PRH_PostService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class postController(ISender sender) : ControllerBase
    {
        [Authorize]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetsPost()
        {
            var response = await sender.Send(new GetsPostQuery());
            return response.ToActionResult();
        }
        [Authorize]
        [HttpPost("add-user-reference")]
        public async Task<IActionResult> AddUserReference(UserPreferenceDto userPreferenceDto)
        {
            var response = await sender.Send(new CreateUserReferenceCommand(userPreferenceDto));
            return response.ToActionResult();
        }
        /// <summary>
        /// Lấy ra các bài viết public user có áp dụng thuật toán recommendation
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("get-homepage")]
        public async Task<IActionResult> GetHomePage(int pageNumber, int pageSize)
        {
            var response = await sender.Send(new GetRecommendedPostsQuery(pageNumber, pageSize));
            return response.ToActionResult();
        }
        /// <summary>
        /// Lấy ra bài viết theo id có thể lấy ra cả bài viết private
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("get-by-post-id/{postId}")]
        public async Task<IActionResult> GetById(string postId)
        {
            var response = await sender.Send(new GetPostsByIdQuery(postId));
            return response.ToActionResult();
        }
        /// <summary>
        /// Api này lấy ra các bài viết của user kể cả bài viết private để cho trang cá nhân của user
        /// </summary>        
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("get-user-post/{userId}")]
        public async Task<IActionResult> GetsUserPost(string userId)
        {
            var response = await sender.Send(new GetsUserPostQuery(userId, 1, 100));
            return response.ToActionResult();
        }
        /// <summary>
        /// Không lấy các bài viết private
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-side-recommendation")]
        public async Task<IActionResult> GetSideRecommendation()
        {
            var response = await sender.Send(new GetSideRecommendPostQuery(PageSize: 1, PageNumber: 7));
            return response.ToActionResult();
        }
        /// <summary>
        /// Lấy ra các bài viết public có lượt reaction cao nhất
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        [HttpGet("get-top-post/{top}")]
        public async Task<IActionResult> GetTopPost(int top)
        {
            var response = await sender.Send(new GetsTopPostQuery(top));
            return response.ToActionResult();
        }
        /// <summary>
        /// Lấy ra các bài viết của một user theo id theo số lượng top
        /// </summary>
        /// <param name="authourId"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        [HttpGet("get-other-authour-post/{authourId}/top")]
        public async Task<IActionResult> GetOtherAuthourPost(string authourId, int top)
        {
            var response = await sender.Send(new GetOtherPostByAutourQuery(authourId, top));
            return response.ToActionResult();
        }
        /// <summary>
        /// Lấy ra các bài viết cùng category với bài viết có id là postId
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        [HttpGet("get-other-relative-post/{postId}/{top}")]
        public async Task<IActionResult> GetOtherRelativePost(string postId, int top)
        {
            var response = await sender.Send(new GetRelativeCatogoryPostQuery(postId, top));
            return response.ToActionResult();
        }
        /// <summary>
        /// Dùng Api get-user-post thay thế vì có paging
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Obsolete]
        [Authorize]
        [HttpGet("get-by-user-id/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var response = await sender.Send(new GetPostsByUserIdQuery(userId));
            return response.ToActionResult();
        }
        /// <summary>
        /// Tạo bài viết mới : Status : 0 = public, 1 = private. Lưu ý: không truyền 2 vào vì 2 là status của bài viết trong group
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("create-post")]
        public async Task<IActionResult> CreatePost(PostDto post)
        {
            var response = await sender.Send(new CreatePostCommand(post, HttpContext));
            return response.ToActionResult();
        }
        /// <summary>
        /// Tao bài viết mới trong group : Status = 2.      Note : Status không cần truyền vào vì luôn luôn là 2
        /// </summary>
        /// <param name="postGroup"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("create-post-group")]
        public async Task<IActionResult> CreatePostInGroup(PostGroupDto postGroup)
        {
            var response = await sender.Send(new CreatePostGroupCommand(postGroup, HttpContext));
            return response.ToActionResult();
        }
        /// <summary>
        /// Lấy danh sách các bài viết trong group - với GroupVisibility = 0 (public) - GroupVisibility = 1 (private) sẽ không thấy
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("get-posts-in-groups")]
        public async Task<IActionResult> GetPostsInGroups()
        {
            var response = await sender.Send(new GetPostsInGroupsQuery(HttpContext));
            return response.ToActionResult();
        }
        /// <summary>
        /// Lấy danh sách các bài viết trong group bằng cách truyền vào GroupId - với GroupVisibility = 0 (public) - GroupVisibility = 1 (private) sẽ không thấy
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("get-posts-in-group-by-id/{groupId}")]
        public async Task<IActionResult> GetPostsInGroupById(string groupId)
        {
            var response = await sender.Send(new GetPostsInGroupByGroupIdQuery(groupId));
            return response.ToActionResult();
        }
        /// <summary>
        /// Lấy ra các bài viết trong group bằng cách truyền vào GroupID và UserID
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("get-posts-in-group-by-user-and-group")]
        public async Task<IActionResult> GetPostsInGroupByUserAndGroup([FromQuery] string groupId, [FromQuery] string userId)
        {
            var response = await sender.Send(new GetPostsInGroupByUserAndGroupQuery(groupId, userId, HttpContext));
            return response.ToActionResult();
        }
        [Authorize]
        [HttpPut("update-post/{id}")]
        public async Task<IActionResult> UpdatePost(string id, PostDto post)
        {
            var response = await sender.Send(new UpdatePostCommand(id, post));
            return response.ToActionResult();
        }
        [Authorize]
        [HttpPut("update-post-in-group/{postId}")]
        public async Task<IActionResult> UpdatePostInGroup(string postId, PostDto postDto)
        {
            var response = await sender.Send(new UpdatePostInGroupCommand(postId, postDto));
            return response.ToActionResult();
        }
        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePost(string id)
        {
            var response = await sender.Send(new DeletePostCommand(id));
            return response.ToActionResult();
        }
    }
}