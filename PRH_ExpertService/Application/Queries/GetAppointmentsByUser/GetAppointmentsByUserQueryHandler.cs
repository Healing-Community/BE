using Application.Commons.DTOs;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;

namespace Application.Queries.GetAppointmentsByUser
{
    public class GetAppointmentsByUserQueryHandler(IAppointmentRepository appointmentRepository)
        : IRequestHandler<GetAppointmentsByUserQuery, BaseResponse<IEnumerable<AppointmentResponseDto>>>
    {
        public async Task<BaseResponse<IEnumerable<AppointmentResponseDto>>> Handle(GetAppointmentsByUserQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<AppointmentResponseDto>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = []
            };

            try
            {
                // Load danh sách lịch hẹn từ repository, bao gồm cả ExpertProfile
                var appointments = await appointmentRepository.GetByUserIdAsync(request.UserId);

                // Map dữ liệu từ Appointment sang AppointmentResponseDto
                var result = appointments.Select(a => new AppointmentResponseDto
                {
                    Name = a.ExpertProfile?.Fullname ?? "Không xác định", // Lấy tên chuyên gia từ DB
                    AppointmentDate = a.AppointmentDate.ToString("yyyy-MM-dd"), // Chuyển DateOnly thành chuỗi
                    TimeRange = $"{a.StartTime:hh\\:mm} - {a.EndTime:hh\\:mm}", // Chuyển TimeOnly thành chuỗi
                    MeetLink = a.MeetLink ?? "", // Giá trị mặc định nếu MeetLink null
                    Tag = MapTag(a.Status, a.AppointmentDate, a.StartTime) // Xử lý tag
                });

                response.Success = true;
                response.Data = result;
                response.StatusCode = 200;
                response.Message = "Lấy danh sách lịch hẹn của người dùng thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy danh sách lịch hẹn.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }

        private string MapTag(int status, DateOnly appointmentDate, TimeOnly startTime)
        {
            var now = DateTime.UtcNow;
            var appointmentDateTime = appointmentDate.ToDateTime(startTime);

            return status switch
            {
                1 => "Chờ thanh toán",
                2 => "Đã thanh toán",
                3 when appointmentDateTime > now => "Sắp diễn ra",
                3 when appointmentDateTime <= now => "Đang diễn ra",
                4 => "Đã hủy",
                5 => "Đã hoàn thành",
                _ => "Không xác định"
            };
        }
    }
}