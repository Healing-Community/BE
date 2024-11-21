﻿using Application.Commands.Comments.AddComment;
using Application.Commands.Comments.DeleteComment;
using Application.Commands.Comments.UpdateComment;
using Application.Commons.DTOs;
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
        [HttpGet("get-all")]
        public async Task<IActionResult> GetComment()
        {
            var response = await sender.Send(new GetCommentsQuery());
            return response.ToActionResult();
        }

        [HttpGet("get-by-comment-id/{commentId}")]
        public async Task<IActionResult> GetById(string commentId)
        {
            var response = await sender.Send(new GetCommentsByIdQuery(commentId));
            return response.ToActionResult();
        }

        [HttpGet("get-by-post-id/{postId}")]
        public async Task<IActionResult> GetCommentsByPostId(string postId)
        {
            var response = await sender.Send(new GetCommentsByPostIdQuery(postId));
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
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateComment(string id, CommentDto comment)
        {
            var response = await sender.Send(new UpdateCommentCommand(id, comment));
            return response.ToActionResult();
        }

        [Authorize(Roles = "User")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteComment(string id)
        {
            var response = await sender.Send(new DeleteCommentCommand(id));
            return response.ToActionResult();
        }
    }
}
