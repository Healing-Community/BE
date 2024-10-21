using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.ReactionTypes.UpdateReactionType
{
    public record UpdateReactionTypeCommand(string reactionTypeId, ReactionTypeDto ReactionTypeDto) : IRequest<BaseResponse<string>>;
}
