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

namespace Application.Queries.ReactionTypes.GetReactionTypesById
{
    public class GetReactionTypesByIdQueryHandler(IReactionTypeRepository reactionTypeRepository)
        : IRequestHandler<GetReactionTypesByIdQuery, BaseResponse<ReactionType>>
    {
        public async Task<BaseResponse<ReactionType>> Handle(GetReactionTypesByIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<ReactionType>()
            {
                Id = NewId.NextSequentialGuid(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                var reactionType = await reactionTypeRepository.GetByIdAsync(request.Id);
                if (reactionType == null)
                {
                    response.Success = false;
                    response.Message = "Reaction type not found";
                    response.Errors.Add("No reaction type found with the provided ID.");
                    return response;
                }
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Data = reactionType;
                response.Success = true;
                response.Message = "Reaction type retrieved successfully";
            }
            catch (Exception e)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "An error occurred";
                response.Errors.Add(e.Message);
            }
            return response;
        }
    }
}
