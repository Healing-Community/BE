using MediatR;
using Application.Commons.DTOs;
using Application.Commons;

namespace Application.Queries.GetExpertList
{
    public record GetExpertListQuery(int PageNumber, int PageSize) : IRequest<BaseResponse<IEnumerable<ExpertListDTO>>>;
}
