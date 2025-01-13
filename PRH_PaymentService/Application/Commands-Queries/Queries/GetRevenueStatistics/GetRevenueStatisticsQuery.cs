using MediatR;
using Application.Commons.DTOs;
using Application.Commons;

namespace Application.Commands_Queries.Queries.GetRevenueStatistics
{
    public class GetRevenueStatisticsQuery : IRequest<BaseResponse<RevenueStatisticsDto>>
    {
    }
}
