using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Security.Claims;

namespace Application.Commands.Reactions.DeleteReaction
{
    public class DeleteReactionCommandHandler(IReactionRepository reactionRepository, IHttpContextAccessor accessor) : IRequestHandler<DeleteReactionCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(DeleteReactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = accessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return BaseResponse<string>.Unauthorized();
                }
                var reaction = await reactionRepository.GetByPropertyAsync(x => x.PostId == request.RemoveReactionDto.PostId && x.UserId == userId);
                if (reaction == null)
                {
                    return BaseResponse<string>.NotFound("Reaction not found");
                }
                await reactionRepository.DeleteAsync(reaction.ReactionId);
                return BaseResponse<string>.SuccessReturn("Xóa reaction thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse<string>.InternalServerError(ex.Message);
                throw;
            }
        }
    }
}
