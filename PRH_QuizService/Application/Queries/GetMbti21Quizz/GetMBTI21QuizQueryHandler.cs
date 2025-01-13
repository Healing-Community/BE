using Application.Commons;
using Domain.Entities.MBTI21;
using MediatR;

namespace Application.Queries.GetMbti21Quizz
{
    public class GetMBTI21QuizQueryHandler(IMongoRepository<Mbti21> mongoRepository)
        : IRequestHandler<GetMBTI21QuizQuery, BaseResponse<Mbti21>>
    {
        public async Task<BaseResponse<Mbti21>> Handle(GetMBTI21QuizQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var quiz = await mongoRepository.GetsAsync();
                if (quiz == null || !quiz.Any())
                    return BaseResponse<Mbti21>.NotFound("Không tìm thấy bài kiểm tra MBTI-21.");
                return BaseResponse<Mbti21>.SuccessReturn(quiz.First());
            }
            catch (Exception ex)
            {
                return BaseResponse<Mbti21>.InternalServerError($"Đã xảy ra lỗi: {ex.Message}");
            }
        }
    }
}
