using Application.Commons.DTOs;
using Application.Commons;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commads_Queries.Commands.Reactions.AddReactionForShare
{
    public record CreateReactionForShareCommand(ReactionShareDto ReactionShareDto, HttpContext HttpContext) : IRequest<BaseResponse<string>>;
}
