using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.Reactions.DeleteReaction
{
     public record DeleteReactionCommand(RemoveReactionDto RemoveReactionDto) : IRequest<BaseResponse<string>>;
}
