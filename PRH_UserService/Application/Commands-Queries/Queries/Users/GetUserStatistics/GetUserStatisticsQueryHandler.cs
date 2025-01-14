using Application.Commons;
using Application.Commons.DTOs;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;

namespace Application.Commands_Queries.Queries.Users.GetUserStatistics;

public class GetUserStatisticsQueryHandler(IUserRepository userRepository) : IRequestHandler<GetUserStatisticsQuery, BaseResponse<UserStatisticsDto>>
{
    public async Task<BaseResponse<UserStatisticsDto>> Handle(GetUserStatisticsQuery request, CancellationToken cancellationToken)
    {
        var response = new BaseResponse<UserStatisticsDto>
        {
            Id = Ulid.NewUlid().ToString(),
            Timestamp = DateTime.UtcNow
        };

        try
        {
            var totalUsers = await userRepository.CountAsync();
            var newUsersThisMonth = await userRepository.CountNewUsersThisMonthAsync();
            var userRolesCount = await userRepository.CountUsersByRoleAsync();

            response.Data = new UserStatisticsDto
            {
                TotalUsers = totalUsers,
                NewUsersThisMonth = newUsersThisMonth,
                UserRolesCount = userRolesCount
            };

            response.Success = true;
            response.StatusCode = 200;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Đã xảy ra lỗi khi lấy thống kê người dùng.";
            response.StatusCode = 500;
            response.Errors = new List<string> { ex.Message };
        }

        return response;
    }
}
