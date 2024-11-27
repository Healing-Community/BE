using Application.Commands.Posts.AddPost;
using Application.Commands.Posts.AddPostGroup;
using Application.Commands.Posts.DeletePost;
using Application.Commands.Posts.UpdatePost;
using Application.Commands.UserReference;
using Application.Commons.DTOs;
using Application.Queries.Posts.GetPosts;
using Application.Queries.Posts.GetPostsById;
using Application.Queries.Posts.GetPostsByUserId;
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
        [HttpGet("get-homepage")]
        public async Task<IActionResult> GetHomePage(int pageNumber, int pageSize)
        {
            var response = await sender.Send(new GetRecommendedPostsQuery(pageNumber, pageSize));
            return response.ToActionResult();
        }
        [HttpGet("get-by-post-id/{postId}")]
        public async Task<IActionResult> GetById(string postId)
        {
            var response = await sender.Send(new GetPostsByIdQuery(postId));
            return response.ToActionResult();
        }
        [HttpGet("get-side-recommendation")]
        public async Task<IActionResult> GetSideRecommendation()
        {
            var response = await sender.Send(new GetSideRecommendPostQuery(PageSize:1, PageNumber:7));
            return response.ToActionResult();
        }
        [HttpGet("get-by-user-id/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var response = await sender.Send(new GetPostsByUserIdQuery(userId));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpPost("create-post")]
        public async Task<IActionResult> CreatePost(PostDto post)
        {
            var response = await sender.Send(new CreatePostCommand(post, HttpContext));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpPost("create-post-group")]
        public async Task<IActionResult> CreatePostInGroup(PostGroupDto postGroup)
        {
            var response = await sender.Send(new CreatePostGroupCommand(postGroup, HttpContext));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpPut("update-post/{id}")]
        public async Task<IActionResult> UpdatePost(string id, PostDto post)
        {
            var response = await sender.Send(new UpdatePostCommand(id, post));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePost(string id)
        {
            var response = await sender.Send(new DeletePostCommand(id));
            return response.ToActionResult();
        }

    }
}
