using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Reactions.DeleteReaction
{
    public class DeleteReactionCommandHandler(IReactionRepository reactionRepository) : IRequestHandler<DeleteReactionCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(DeleteReactionCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = request.Id,
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                await reactionRepository.DeleteAsync(request.Id);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Reaction deleted successfully";
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Failed to delete reaction";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
