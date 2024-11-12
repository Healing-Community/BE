using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using System.Net;

namespace Application.Commands.Reactions.UpdateReaction
{
    public class UpdateReactionCommandHandler(IReactionRepository reactionRepository)
        : IRequestHandler<UpdateReactionCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(UpdateReactionCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = request.reactionId,
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };
            try
            {
                var existingReaction = await reactionRepository.GetByIdAsync(request.reactionId);
                var updatedReaction = new Reaction
                {
                    ReactionId = request.reactionId,
                    ReactionTypeId = request.ReactionDto.ReactionTypeId,
                    CreateAt = existingReaction.CreateAt,
                    UpdateAt = DateTime.UtcNow.AddHours(7),
                };
                await reactionRepository.Update(request.reactionId, updatedReaction);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Cập nhật thành công";
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Lỗi !!! Cập nhật thất bại";
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}
