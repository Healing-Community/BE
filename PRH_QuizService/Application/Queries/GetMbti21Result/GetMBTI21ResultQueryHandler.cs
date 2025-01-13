using Application.Commons.Tools;
using Application.Commons;
using Domain.Entities.MBTI21;
using MediatR;

namespace Application.Queries.GetMbti21Result
{
    public class GetMBTI21ResultQueryHandler(IMongoRepository<Mbti21Result> mongoRepository)
        : IRequestHandler<GetMBTI21ResultQuery, BaseResponse<Mbti21Result>>
    {
        public async Task<BaseResponse<Mbti21Result>> Handle(GetMBTI21ResultQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = Authentication.GetUserIdFromHttpContext(request.HttpContext);
                var result = await mongoRepository.GetByPropertyAsync(x => x.UserId == userId);
                return result != null
                    ? BaseResponse<Mbti21Result>.SuccessReturn(result)
                    : BaseResponse<Mbti21Result>.NotFound();
            }
            catch (Exception ex)
            {
                return BaseResponse<Mbti21Result>.InternalServerError(ex.Message);
            }
        }
    }
}
