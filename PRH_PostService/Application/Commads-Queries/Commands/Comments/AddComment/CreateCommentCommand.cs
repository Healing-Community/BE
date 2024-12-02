using Application.Commons;
using Application.Commons.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Comments.AddComment
{
    public record CreateCommentCommand(CommentDto CommentDto, HttpContext HttpContext) : IRequest<BaseResponse<string>>;
}
