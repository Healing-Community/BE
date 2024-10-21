using Application.Commons;
using MediatR;

namespace Application.Commands.Reactions.DeleteReaction
{
     public record DeleteReactionCommand(string Id) : IRequest<BaseResponse<string>>;
}
