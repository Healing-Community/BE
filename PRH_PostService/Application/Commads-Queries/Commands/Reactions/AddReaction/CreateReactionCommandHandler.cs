using Application.Commons;
using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using NUlid;
using System.Security.Claims;

namespace Application.Commands.Reactions.AddReaction
{
    public class CreateReactionCommandHandler(
        IReactionTypeRepository reactionTypeRepository,
        IHttpContextAccessor accessor,
        IReactionRepository reactionRepository) : IRequestHandler<CreateReactionCommand, BaseResponse<ReactionType>>
    {
        public async Task<BaseResponse<ReactionType>> Handle(CreateReactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = accessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return BaseResponse<ReactionType>.Unauthorized();
                }

                // Tính thời gian hiện tại (UTC + 7)
                var currentTime = DateTime.UtcNow + TimeSpan.FromHours(7);

                // Kiểm tra xem người dùng đã có reaction với bài viết này chưa
                var existingReaction = await reactionRepository.GetByPropertyAsync(
                    x => x.PostId == request.ReactionDto.PostId && x.UserId == userId);

                // Nếu người dùng đã reaction với loại tương tự, trả về thông báo lỗi
                if (existingReaction?.ReactionTypeId == request.ReactionDto.ReactionTypeId)
                {
                    return BaseResponse<ReactionType>.BadRequest("Bạn đã reaction bài viết này rồi.");
                }

                // Nếu người dùng đã có reaction nhưng với loại khác, cập nhật lại reaction
                if (existingReaction != null && existingReaction.ReactionTypeId != request.ReactionDto.ReactionTypeId)
                {
                    var reactionTypeUpdate = await reactionTypeRepository.GetByPropertyAsync(
                        x => x.ReactionTypeId == request.ReactionDto.ReactionTypeId);

                    if (reactionTypeUpdate == null)
                    {
                        return BaseResponse<ReactionType>.NotFound("Loại reaction không tồn tại.");
                    }

                    existingReaction.ReactionTypeId = request.ReactionDto.ReactionTypeId;
                    existingReaction.UpdateAt = currentTime;

                    await reactionRepository.Update(existingReaction.ReactionId, existingReaction);

                    var reactionTypeIndb = await reactionTypeRepository.GetByPropertyAsync(
                        x => x.ReactionTypeId == existingReaction.ReactionTypeId);
                    if (reactionTypeIndb == null)
                    {
                        return BaseResponse<ReactionType>.NotFound("Loại reaction không tồn tại.");
                    }
                    return BaseResponse<ReactionType>.SuccessReturn(reactionTypeIndb, "Cập nhật phản hồi thành công.");
                }

                // Kiểm tra sự tồn tại của loại reaction
                var reactionType = await reactionTypeRepository.GetByPropertyAsync(
                    x => x.ReactionTypeId == request.ReactionDto.ReactionTypeId);

                if (reactionType == null || reactionType.ReactionTypeId == Ulid.Empty.ToString())
                {
                    return BaseResponse<ReactionType>.NotFound("Loại reaction không tồn tại.");
                }

                // Tạo mới một reaction
                var newReaction = new Reaction
                {
                    ReactionId = Ulid.NewUlid().ToString(),
                    PostId = request.ReactionDto.PostId,
                    ReactionTypeId = request.ReactionDto.ReactionTypeId,
                    UserId = userId,
                    CreateAt = currentTime,
                    UpdateAt = currentTime
                };

                await reactionRepository.Create(newReaction);

                return BaseResponse<ReactionType>.SuccessReturn(reactionType,"Phản hồi bài viết thành công");
            }
            catch (Exception ex)
            {
                return BaseResponse<ReactionType>.InternalServerError($"Đã xảy ra lỗi: {ex.Message}");
            }
        }
    }
}
