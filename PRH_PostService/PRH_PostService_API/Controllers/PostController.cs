using Application.Queries.Posts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace PRH_PostService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController(ISender sender) : ControllerBase
    {
        [HttpGet("GetsPost")]
        public async Task<IActionResult> GetsPost()
        {
            var response = await sender.Send(new GetsPostQuery());
            return Ok(response);
        }
    }
}
