using Application.Commons;
using Domain.Entities.MBTI21;
using MediatR;


namespace Application.Commands.CreateMbti21Quiz
{
    public record CreateMBTI21Command(Mbti21 MBTI21) : IRequest<BaseResponse<bool>>;
}
