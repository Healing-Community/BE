using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.Reactions.AddReaction
{
    public record CreateReactionCommand(ReactionDto ReactionDto) : IRequest<BaseResponse<string>>;
}
