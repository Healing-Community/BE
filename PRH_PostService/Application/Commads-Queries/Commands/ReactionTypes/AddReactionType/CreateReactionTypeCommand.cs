using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.ReactionTypes.AddReactionType
{
    public record CreateReactionTypeCommand(ReactionTypeDto ReactionTypeDto) : IRequest<BaseResponse<string>>;
}
