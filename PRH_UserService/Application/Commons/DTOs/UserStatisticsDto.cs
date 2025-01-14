namespace Application.Commons.DTOs;

public class UserStatisticsDto
{
    public int TotalUsers { get; set; }
    public int NewUsersThisMonth { get; set; }
    public Dictionary<string, int> UserRolesCount { get; set; }
}
