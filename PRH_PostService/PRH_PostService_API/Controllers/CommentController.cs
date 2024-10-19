using Application.Commands.Comments.AddComment;
using Application.Commands.Comments.DeleteComment;
using Application.Commands.Comments.UpdateComment;
using Application.Commons.DTOs;
using Application.Queries.Comments.GetComments;
using Application.Queries.Comments.GetCommentsById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRH_PostService_API.Extentions;

namespace PRH_PostService_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController(ISender sender) : ControllerBase
    {
        [Authorize(Roles = "User")]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetComment()
        {
            var response = await sender.Send(new GetCommentsQuery());
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpGet("get-by-id/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await sender.Send(new GetCommentsByIdQuery(id));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateComment(CommentDto comment)
        {
            var response = await sender.Send(new CreateCommentCommand(comment, HttpContext));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpPut("update/{id:guid}")]
        public async Task<IActionResult> UpdateComment(Guid id, CommentDto comment)
        {
            var response = await sender.Send(new UpdateCommentCommand(id, comment));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpDelete("delete/{id:guid}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var response = await sender.Send(new DeleteCommentCommand(id));
            return response.ToActionResult();
        }
    }
}
