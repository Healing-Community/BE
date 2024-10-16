using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.ReactionTypes.AddReactionType
{
    public class CreateReactionTypeCommandHandler(IReactionTypeRepository reactionTypeRepository) 
        : IRequestHandler<CreateReactionTypeCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(CreateReactionTypeCommand request, CancellationToken cancellationToken)
        {
            var reactionTypeId = NewId.NextSequentialGuid();
            var reactionType = new ReactionType
            {
                ReactionTypeId = reactionTypeId,
                Name = request.ReactionTypeDto.Name,
            };
            var response = new BaseResponse<string>
            {
                Id = reactionTypeId,
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                await reactionTypeRepository.Create(reactionType);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Reaction type created successfully";
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Failed to create reaction type";
                response.Errors.Add(ex.Message);
            }

            return response;
        }
    }
}
