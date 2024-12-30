using Application.Commons.DTOs;
using Application.Commons;
using MediatR;

namespace Application.Commads_Queries.Commands.Posts.UpdatePostInGroup
{
    public record UpdatePostInGroupCommand(string PostId, PostDto PostDto) : IRequest<BaseResponse<string>>;
}
