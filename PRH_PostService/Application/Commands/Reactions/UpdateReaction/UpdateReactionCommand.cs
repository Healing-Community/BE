using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.Reactions.UpdateReaction
{
    public record UpdateReactionCommand(Guid reactionId, ReactionDto ReactionDto) : IRequest<BaseResponse<string>>;
}
