using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;
using System.Security.Claims;


namespace Application.Commads_Queries.Commands.Reactions.AddReactionForShare
{
    public class CreateReactionForShareCommandHandler(
            IReactionRepository reactionRepository,
            IReactionTypeRepository reactionTypeRepository,
            IHttpContextAccessor accessor
        ) : IRequestHandler<CreateReactionForShareCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateReactionForShareCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = accessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return BaseResponse<string>.Unauthorized();
                }

                // Kiểm tra xem người dùng đã reaction với ShareId này chưa
                var existingReaction = await reactionRepository.GetByPropertyAsync(
                    x => x.ShareId == request.ReactionShareDto.ShareId && x.UserId == userId);

                if (existingReaction?.ReactionTypeId == request.ReactionShareDto.ReactionTypeId)
                {
                    return BaseResponse<string>.BadRequest("Bạn đã reaction bài viết này rồi.");
                }

                // Nếu có reaction khác, cập nhật loại reaction
                if (existingReaction != null && existingReaction.ReactionTypeId != request.ReactionShareDto.ReactionTypeId)
                {
                    existingReaction.ReactionTypeId = request.ReactionShareDto.ReactionTypeId;
                    existingReaction.UpdateAt = DateTime.UtcNow.AddHours(7);
                    await reactionRepository.Update(existingReaction.ReactionId, existingReaction);
                    return BaseResponse<string>.SuccessReturn("Cập nhật reaction thành công.");
                }

                // Kiểm tra loại reaction có tồn tại không
                var reactionType = await reactionTypeRepository.GetByPropertyAsync(
                    x => x.ReactionTypeId == request.ReactionShareDto.ReactionTypeId);

                if (reactionType == null)
                {
                    return BaseResponse<string>.NotFound("Loại reaction không tồn tại.");
                }

                // Tạo reaction mới
                var newReaction = new Reaction
                {
                    ReactionId = Ulid.NewUlid().ToString(),
                    ShareId = request.ReactionShareDto.ShareId,
                    ReactionTypeId = request.ReactionShareDto.ReactionTypeId,
                    UserId = userId,
                    CreateAt = DateTime.UtcNow.AddHours(7),
                    UpdateAt = DateTime.UtcNow.AddHours(7)
                };

                await reactionRepository.Create(newReaction);
                return BaseResponse<string>.SuccessReturn("Tạo reaction thành công.");
            }
            catch (Exception ex)
            {
                return BaseResponse<string>.InternalServerError($"Đã xảy ra lỗi: {ex.Message}");
            }
        }
    }
}
