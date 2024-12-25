using Application.Commons;
using Application.Commons.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;


namespace Application.Commads_Queries.Commands.Comments.CreateCommentForShare
{
    public record CreateCommentForShareCommand(CreateCommentForShareDto CommentDto, HttpContext HttpContext)
        : IRequest<BaseResponse<CommentDtoResponse>>;
}
