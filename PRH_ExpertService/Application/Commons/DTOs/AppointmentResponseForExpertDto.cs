namespace Application.Commons.DTOs
{
    public class AppointmentResponseForExpertDto
    {
        public string Name { get; set; } // Email hoặc tên của người dùng
        public string AppointmentDate { get; set; } // Ngày hẹn (yyyy-MM-dd)
        public string TimeRange { get; set; } // Thời gian hẹn (hh:mm - hh:mm)
        public string MeetLink { get; set; } // Link họp
        public string Tag { get; set; } // Tag hiển thị (Sắp diễn ra, Đã hoàn thành)
        public string UserId { get; set; } // ID của người dùng
    }
}
