using Application.Commons;
using Application.Commons.DTOs;
using MediatR;

namespace Application.Commands.Reactions.DeleteReaction
{
     public record DeleteReactionCommand(PostIdOnlyDto RemoveReactionDto) : IRequest<BaseResponse<string>>;
}
