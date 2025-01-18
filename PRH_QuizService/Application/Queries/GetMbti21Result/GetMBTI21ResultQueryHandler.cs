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

                if (result == null)
                {
                    return BaseResponse<Mbti21Result>.NotFound();
                }

                // Đảm bảo map các giá trị điểm số từ cơ sở dữ liệu vào kết quả
                var response = new Mbti21Result
                {
                    Id = result.Id,
                    UserId = result.UserId,
                    DateTaken = result.DateTaken,
                    ExtroversionScore = result.ExtroversionScore,
                    SensingScore = result.SensingScore,
                    ThinkingScore = result.ThinkingScore,
                    JudgingScore = result.JudgingScore,
                    ExtroversionDescription = result.ExtroversionDescription,
                    SensingDescription = result.SensingDescription,
                    ThinkingDescription = result.ThinkingDescription,
                    JudgingDescription = result.JudgingDescription,
                    OverallComment = result.OverallComment,
                    Factors = result.Factors,
                    ShortTermEffects = result.ShortTermEffects,
                    LongTermEffects = result.LongTermEffects
                };

                return BaseResponse<Mbti21Result>.SuccessReturn(response);
            }
            catch (Exception ex)
            {
                return BaseResponse<Mbti21Result>.InternalServerError(ex.Message);
            }
        }
    }
}
