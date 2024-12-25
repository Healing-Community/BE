using MediatR;
using Application.Commons.DTOs;
using Application.Commons;

namespace Application.Queries.GetExpertList
{
    public record GetExpertListQuery() : IRequest<BaseResponse<IEnumerable<ExpertListDTO>>>;
}