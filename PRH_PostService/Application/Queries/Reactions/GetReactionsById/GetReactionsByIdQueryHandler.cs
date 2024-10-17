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

namespace Application.Queries.Reactions.GetReactionsById
{
    public class GetReactionsByIdQueryHandler(IReactionRepository reactionRepository) 
        : IRequestHandler<GetReactionsByIdQuery, BaseResponse<Reaction>>
    {
        public async Task<BaseResponse<Reaction>> Handle(GetReactionsByIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<Reaction>()
            {
                Id = NewId.NextSequentialGuid(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                var reaction = await reactionRepository.GetByIdAsync(request.Id);
                if (reaction == null)
                {
                    response.Success = false;
                    response.Message = "Reaction not found";
                    response.Errors.Add("No reaction found with the provided ID.");
                    return response;
                }
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Data = reaction;
                response.Success = true;
                response.Message = "Reaction retrieved successfully";
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
