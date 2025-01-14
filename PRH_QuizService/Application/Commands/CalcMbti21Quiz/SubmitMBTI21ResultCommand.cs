using Application.Commons.DTOs;
using Application.Commons;
using Domain.Entities.MBTI21;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.CalcMbti21Quiz
{
    public record SubmitMBTI21ResultCommand(MBTIQuizzResultRequest MBTIQuizzResultRequest, HttpContext HttpContext) : IRequest<BaseResponse<Mbti21Result>>;
}
