namespace Application.Commons.DTOs
{
    public class RevenueStatisticsDto
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? Day { get; set; }
        public string? DayOfWeek { get; set; }
        public int? WeekOfMonth { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalBookings { get; set; }
    }
}
