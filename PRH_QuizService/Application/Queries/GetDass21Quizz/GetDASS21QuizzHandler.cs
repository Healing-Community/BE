using Application.Commons;
using MediatR;
using Domain.Entities.DASS21;
namespace Application.Queries.GetDass22Quizz
{
    public class GetDASS21QuizzHandler(IMongoRepository<Dass21> mongoRepository) : IRequestHandler<GetDASS21Quizz, BaseResponse<Dass21>>
    {
        public async Task<BaseResponse<Dass21>> Handle(GetDASS21Quizz request, CancellationToken cancellationToken)
        {
            try
            {
                var dass21 = await mongoRepository.GetsAsync();
                if (dass21 == null)
                {
                    return BaseResponse<Dass21>.NotFound();
                }
                else
                {
                    return BaseResponse<Dass21>.SuccessReturn(dass21.First());
                }

            }
            catch (Exception ex)
            {
                return BaseResponse<Dass21>.InternalServerError(ex.Message);
            }
        }
    }
}
