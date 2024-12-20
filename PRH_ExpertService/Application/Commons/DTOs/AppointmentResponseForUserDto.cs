namespace Application.Commons.DTOs
{
    public class AppointmentResponseForUserDto
    {
        public string Name { get; set; } // Tên chuyên gia
        public string AppointmentDate { get; set; } // Ngày hẹn (yyyy-MM-dd)
        public string TimeRange { get; set; }
        public string MeetLink { get; set; }
        public string Tag { get; set; } // Tag hiển thị (Sắp diễn ra, Đã hoàn thành)
        public string ExpertId { get; set; }
        public string AppointmentId { get; set; }
        public int Amount { get; set; }
    }
}
