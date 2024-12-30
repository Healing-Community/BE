using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace Application.Commads_Queries.Commands.Reactions.DeleteReactionByShareId
{
    public class DeleteReactionByShareIdCommandHandler(IReactionRepository reactionRepository, IHttpContextAccessor accessor)
        : IRequestHandler<DeleteReactionByShareIdCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(DeleteReactionByShareIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = accessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return BaseResponse<string>.Unauthorized();
                }
                // Tìm reaction bằng ShareId và UserId
                var reaction = await reactionRepository.GetByPropertyAsync(x => x.ShareId == request.RemoveReactionDto.ShareId && x.UserId == userId);
                if (reaction == null)
                {
                    return BaseResponse<string>.NotFound("Không tìm thấy Reaction nào liên quan đến bài viết được chia sẻ.");
                }
                await reactionRepository.DeleteAsync(reaction.ReactionId);
                return BaseResponse<string>.SuccessReturn("Xóa reaction thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse<string>.InternalServerError(ex.Message);
            }
        }
    }

}
