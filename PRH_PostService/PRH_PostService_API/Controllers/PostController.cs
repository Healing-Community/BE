using Application.Commands.Posts.AddPost;
using Application.Commands.Posts.DeletePost;
using Application.Commands.Posts.UpdatePost;
using Application.Commons.DTOs;
using Application.Queries.Posts.GetPosts;
using Application.Queries.Posts.GetPostsById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
            return Ok(response);
        }

        [HttpGet("get-by-id/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var post = await sender.Send(new GetPostsByIdQuery(id));
            return Ok(post);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePost(PostDto post)
        {
            var addedPost = await sender.Send(new CreatePostCommand(post));
            return Ok(addedPost);
        }

        [HttpPut("update/{id:guid}")]
        public async Task<IActionResult> UpdatePost(Guid id, PostDto post)
        {
            var updatedPost = await sender.Send(new UpdatePostCommand(id, post));
            return Ok(updatedPost);
        }

        [HttpDelete("delete/{id:guid}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var response = await sender.Send(new DeletePostCommand(id));
            return Ok(response);
        }

    }
}
