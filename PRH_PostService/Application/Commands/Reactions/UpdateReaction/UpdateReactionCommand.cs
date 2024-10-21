using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.Reactions.UpdateReaction
{
    public record UpdateReactionCommand(string reactionId, ReactionDto ReactionDto) : IRequest<BaseResponse<string>>;
}
