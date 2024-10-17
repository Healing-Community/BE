using Application.Commons;
using MediatR;

namespace Application.Commands.Reactions.DeleteReaction
{
     public record DeleteReactionCommand(Guid Id) : IRequest<BaseResponse<string>>;
}
