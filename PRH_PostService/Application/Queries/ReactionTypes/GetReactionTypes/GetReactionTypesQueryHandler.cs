using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MassTransit;
using MediatR;
using System.Net;

namespace Application.Queries.ReactionTypes.GetReactionTypes
{
    public class GetReactionTypesQueryHandler(IReactionTypeRepository reactionTypeRepository)
        : IRequestHandler<GetReactionTypesQuery, BaseResponse<IEnumerable<ReactionType>>>
    {
        public async Task<BaseResponse<IEnumerable<ReactionType>>> Handle(GetReactionTypesQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<ReactionType>>()
            {
                Id = NewId.NextSequentialGuid(),
                Timestamp = DateTime.UtcNow,
                Errors = new List<string>()
            };
            try
            {
                var reactionTypes = await reactionTypeRepository.GetsAsync();
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Message = "Lấy dữ liệu thành công";
                response.Success = true;
                response.Data = reactionTypes;
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
