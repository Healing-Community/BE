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
    public class CreateReactionCommandHandler(IReactionTypeRepository reactionTypeRepository,IHttpContextAccessor accessor,IMessagePublisher messagePublisher, IReactionRepository reactionRepository)
        : IRequestHandler<CreateReactionCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateReactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = accessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var reactionInDb = await reactionRepository.GetByPropertyAsync(x => x.PostId == request.ReactionDto.PostId && x.UserId == userId);
                if (reactionInDb != null)
                {
                    return BaseResponse<string>.BadRequest("Bạn đã reaction bài viết này rồi");
                }
                var reactionType = await reactionTypeRepository.GetByPropertyAsync(x => x.ReactionTypeId == request.ReactionDto.ReactionTypeId);
                if (reactionType?.ReactionTypeId == Ulid.Empty.ToString())
                {
                    return BaseResponse<string>.NotFound("Loại reaction không tồn tại");
                }
                var reactionRecord = new Reaction
                {
                    ReactionId = Ulid.NewUlid().ToString(),
                    PostId = request.ReactionDto.PostId,
                    ReactionTypeId = request.ReactionDto.ReactionTypeId,
                    UserId = userId,
                    CreateAt = DateTime.UtcNow + TimeSpan.FromHours(7),
                    UpdateAt = DateTime.UtcNow + TimeSpan.FromHours(7)
                };
                await reactionRepository.Create(reactionRecord);
                return BaseResponse<string>.SuccessReturn($"{reactionType?.Name} bài viết thành công.");
            }
            catch (Exception ex)
            {
                return BaseResponse<string>.InternalServerError(ex.Message);
                throw;
            }
        }
    }
}
