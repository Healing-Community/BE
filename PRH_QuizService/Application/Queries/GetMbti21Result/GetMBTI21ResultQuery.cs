using Application.Commons;
using Domain.Entities.MBTI21;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Queries.GetMbti21Result
{
    public record GetMBTI21ResultQuery(HttpContext HttpContext) : IRequest<BaseResponse<Mbti21Result>>;

}
