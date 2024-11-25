namespace Application.Commons.DTOs
{
    public class AppointmentResponseForUserDto
    {
        public string Name { get; set; } // Tên chuyên gia
        public string AppointmentDate { get; set; } // Ngày hẹn (yyyy-MM-dd)
        public string TimeRange { get; set; } // Thời gian hẹn (hh:mm - hh:mm)
        public string MeetLink { get; set; } // Link họp
        public string Tag { get; set; } // Tag hiển thị (Sắp diễn ra, Đã hoàn thành)
        public string ExpertId { get; set; } // ID của chuyên gia
    }
}
