using Application.Commons;
using Domain.Entities.MBTI21;
using MediatR;

namespace Application.Queries.GetMbti21Quizz
{
    public record GetMBTI21QuizQuery : IRequest<BaseResponse<Mbti21>>;

}
