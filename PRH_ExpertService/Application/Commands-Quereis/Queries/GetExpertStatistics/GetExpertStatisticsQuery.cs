using MediatR;
using Application.Commons.DTOs;
using Application.Commons;

namespace Application.Queries.GetExpertStatistics
{
    public record GetExpertStatisticsQuery() : IRequest<BaseResponse<ExpertStatisticsDTO>>;
}
