using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using System.Net;

namespace Application.Commands.ReactionTypes.UpdateReactionType
{
    public class UpdateReactionTypeCommandHandler(IReactionTypeRepository reactionTypeRepository)
        : IRequestHandler<UpdateReactionTypeCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(UpdateReactionTypeCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = request.reactionTypeId,
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };
            try
            {
                var existingReaction = await reactionTypeRepository.GetByIdAsync(request.reactionTypeId);
                var updatedReaction = new ReactionType
                {
                    ReactionTypeId = request.reactionTypeId,
                    Name = request.ReactionTypeDto.Name,
                };
                await reactionTypeRepository.Update(request.reactionTypeId, updatedReaction);
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
