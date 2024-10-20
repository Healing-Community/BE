using Application.Commons;
using MediatR;

namespace Application.Commands.ReactionTypes.DeleteReactionType
{
    public record DeleteReactionTypeCommand(Guid Id) : IRequest<BaseResponse<string>>;
}
