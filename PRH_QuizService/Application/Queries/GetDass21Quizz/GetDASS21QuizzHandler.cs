using Application.Commons;
using MediatR;
using Domain.Entities.DASS21;
using MassTransit;
namespace Application.Queries.GetDass22Quizz
{
    public class GetDASS21QuizzHandler(IMongoRepository<Dass21> mongoRepository) : IRequestHandler<GetDASS21Quizz, BaseResponse<Dass21>>
    {
        public async Task<BaseResponse<Dass21>> Handle(GetDASS21Quizz request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<Dass21>
            {
                Id = NewId.NextSequentialGuid(),
                Timestamp = DateTime.UtcNow
            };
            try
            {
                var dass21 = await mongoRepository.GetsAsync();
                response.Data = dass21.FirstOrDefault();
                response.Success = true;
                response.StatusCode = 200;
                response.Message = "DASS21 Quizz retrieved successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.StatusCode = 500;
                response.Message = "An error occurred while retrieving DASS21 Quizz";
                response.Errors = new List<string> { ex.Message };
                return response;
            }
        }
    }
}
