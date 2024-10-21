using Application.Commons;
using MediatR;

namespace Application.Commands.ReactionTypes.DeleteReactionType
{
    public record DeleteReactionTypeCommand(string Id) : IRequest<BaseResponse<string>>;
}
