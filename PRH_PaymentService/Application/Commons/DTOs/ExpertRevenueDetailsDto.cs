namespace Application.Commons.DTOs;

public class ExpertRevenueDetailsDto
{
    public int? Year { get; set; }
    public int? Month { get; set; }
    public int? Day { get; set; }
    public string? DayOfWeek { get; set; }
    public int? WeekOfMonth { get; set; }
    public int TotalRevenue { get; set; }
    public int TotalBookings { get; set; }
}
