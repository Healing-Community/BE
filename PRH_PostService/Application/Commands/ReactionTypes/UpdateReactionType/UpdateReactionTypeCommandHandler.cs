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
                Timestamp = DateTime.UtcNow,
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
                response.Message = "Reaction type updated successfully";
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Failed to update reaction type";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
