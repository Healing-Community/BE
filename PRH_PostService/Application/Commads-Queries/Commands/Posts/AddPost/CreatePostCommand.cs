using Application.Commons;
using Application.Commons.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Posts.AddPost
{
    public record CreatePostCommand(PostDto PostDto, HttpContext HttpContext) : IRequest<BaseResponse<string>>;
}
