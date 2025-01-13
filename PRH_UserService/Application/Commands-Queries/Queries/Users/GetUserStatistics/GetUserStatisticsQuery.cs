using MediatR;
using Application.Commons.DTOs;
using Application.Commons;

namespace Application.Commands_Queries.Queries.Users.GetUserStatistics;

public class GetUserStatisticsQuery : IRequest<BaseResponse<UserStatisticsDto>>
{
}
