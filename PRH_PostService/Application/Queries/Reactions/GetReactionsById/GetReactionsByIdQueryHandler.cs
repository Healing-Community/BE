using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using System.Net;

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
                    response.Message = "Không tìm thấy";
                    response.Errors.Add("Không tìm thấy dữ liệu nào có ID được cung cấp.");
                    return response;
                }
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Data = reaction;
                response.Success = true;
                response.Message = "Lấy dữ liệu thành công";
            }
            catch (Exception e)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Success = false;
                response.Message = "Có lỗi xảy ra";
                response.Errors.Add(e.Message);
            }
            return response;
        }
    }
}
