using Application.Commons;
using Application.Commons.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Posts.AddPostGroup
{
    public record CreatePostGroupCommand(PostGroupDto PostGroupDto, HttpContext HttpContext) : IRequest<BaseResponse<string>>;
}
