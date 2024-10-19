using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.ReactionTypes.DeleteReactionType
{
    public class DeleteReactionTypeCommandHandler(IReactionTypeRepository reactionTypeRepository) 
        : IRequestHandler<DeleteReactionTypeCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(DeleteReactionTypeCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = request.Id,
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                await reactionTypeRepository.DeleteAsync(request.Id);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Success = true;
                response.Message = "Xoá thành công";
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Lỗi !!! Xoá thất bại";
                response.Errors.Add(ex.Message);
            }
            return response;
        }
    }
}
