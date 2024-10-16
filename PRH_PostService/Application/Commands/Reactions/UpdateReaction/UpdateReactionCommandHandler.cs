using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                var existingReaction = await reactionRepository.GetByIdAsync(request.reactionId);
                var updatedReaction = new Reaction
                {
                    ReactionId = request.reactionId,
                    ReactionTypeId = request.ReactionDto.ReactionTypeId
                };
                await reactionRepository.Update(request.reactionId, updatedReaction);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Reaction updated successfully";
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Failed to update reaction";
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}
