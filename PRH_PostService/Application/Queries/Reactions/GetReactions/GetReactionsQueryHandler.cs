using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using System.Net;

namespace Application.Queries.Reactions.GetReactions
{
    public class GetReactionsQueryHandler(IReactionRepository reactionRepository) 
        : IRequestHandler<GetReactionsQuery, BaseResponse<IEnumerable<Reaction>>>
    {
        public async Task<BaseResponse<IEnumerable<Reaction>>> Handle(GetReactionsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<Reaction>>()
            {
                Id = NewId.NextSequentialGuid(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                var reactions = await reactionRepository.GetsAsync();
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Message = "Lấy dữ liệu thành công";
                response.Success = true;
                response.Data = reactions;
            }
            catch (Exception e)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = e.Message;
                response.Success = false;
            }
            return response;
        }
    }
}
