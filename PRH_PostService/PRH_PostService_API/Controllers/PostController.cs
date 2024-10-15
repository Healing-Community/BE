using Application.Commands.Posts.AddPost;
using Application.Commands.Posts.DeletePost;
using Application.Commands.Posts.UpdatePost;
using Application.Commons.DTOs;
using Application.Queries.Posts.GetPosts;
using Application.Queries.Posts.GetPostsById;
using MassTransit;
using MediatR;
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

        [HttpGet("get-by-id/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await sender.Send(new GetPostsByIdQuery(id));
            return response.ToActionResult();
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePost(PostDto post)
        {
            var response = await sender.Send(new CreatePostCommand(post));
            return response.ToActionResult();
        }

        [HttpPut("update/{id:guid}")]
        public async Task<IActionResult> UpdatePost(Guid id, PostDto post)
        {
            var response = await sender.Send(new UpdatePostCommand(id, post));
            return response.ToActionResult();
        }

        [HttpDelete("delete/{id:guid}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var response = await sender.Send(new DeletePostCommand(id));
            return response.ToActionResult();
        }

    }
}
