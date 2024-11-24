namespace Application.Commons.DTOs
{
    public class AppointmentResponseDto
    {
        public string Name { get; set; } // Tên User hoặc Expert
        public string AppointmentDate { get; set; } // Ngày hẹn (yyyy-MM-dd)
        public string TimeRange { get; set; } // Thời gian hẹn (hh:mm - hh:mm)
        public string MeetLink { get; set; } // Link họp
        public string Tag { get; set; } // Tag hiển thị (Sắp diễn ra, Đã hoàn thành)
    }
}