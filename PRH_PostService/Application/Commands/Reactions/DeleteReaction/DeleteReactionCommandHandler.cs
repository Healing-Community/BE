using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using System.Net;

namespace Application.Commands.Reactions.DeleteReaction
{
    public class DeleteReactionCommandHandler(IReactionRepository reactionRepository) : IRequestHandler<DeleteReactionCommand, BaseResponse<string>>
    {
        public async Task<BaseResponse<string>> Handle(DeleteReactionCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<string>
            {
                Id = request.Id,
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };
            try
            {
                await reactionRepository.DeleteAsync(request.Id);
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
