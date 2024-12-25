using Application.Commads_Queries.Commands.Comments.CreateCommentForShare;
using Application.Commads_Queries.Queries.Comments.GetCommentsByShareId;
using Application.Commands.Comments.AddComment;
using Application.Commands.Comments.DeleteComment;
using Application.Commands.Comments.UpdateComment;
using Application.Commons.DTOs;
using Application.Queries.Comments.CountTotalCommentByPostId;
using Application.Queries.Comments.GetComments;
using Application.Queries.Comments.GetCommentsById;
using Application.Queries.Comments.GetCommentsByPostId;
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
        [AllowAnonymous]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetComment()
        {
            var response = await sender.Send(new GetCommentsQuery());
            return response.ToActionResult();
        }

        [AllowAnonymous]
        [HttpGet("get-by-comment-id/{commentId}")]
        public async Task<IActionResult> GetById(string commentId)
        {
            var response = await sender.Send(new GetCommentsByIdQuery(commentId));
            return response.ToActionResult();
        }

        [AllowAnonymous]
        [HttpGet("get-by-post-id/{postId}")]
        public async Task<IActionResult> GetCommentsByPostId(string postId)
        {
            var response = await sender.Send(new GetCommentsByPostIdQuery(postId));
            return response.ToActionResult();
        }

        [AllowAnonymous]
        [HttpGet("get-by-share-id/{shareId}")]
        public async Task<IActionResult> GetCommentsByShareId(string shareId)
        {
            var response = await sender.Send(new GetCommentsByShareIdQuery(shareId));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User, Expert")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateComment(CommentDto comment)
        {
            var response = await sender.Send(new CreateCommentCommand(comment, HttpContext));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User, Expert")]
        [HttpPost("create-for-share")]
        public async Task<IActionResult> CreateCommentForShare(CreateCommentForShareDto commentDto)
        {
            var response = await sender.Send(new CreateCommentForShareCommand(commentDto, HttpContext));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User, Expert")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateComment(string id, CommentDto comment)
        {
            var response = await sender.Send(new UpdateCommentCommand(id, comment));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User, Expert")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteComment(string id)
        {
            var response = await sender.Send(new DeleteCommentCommand(id));
            return response.ToActionResult();
        }

        [AllowAnonymous]
        [HttpGet("count-by-post-id/{postId}")]
        public async Task<IActionResult> CountTotalCommentByPostId(string postId)
        {
            var response = await sender.Send(new CountTotalCommentByPostIdQuery(postId));
            return response.ToActionResult();
        }
    }
}
