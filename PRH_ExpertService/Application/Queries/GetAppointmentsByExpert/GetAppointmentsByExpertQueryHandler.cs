using Application.Commons.DTOs;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;

namespace Application.Queries.GetAppointmentsByExpert
{
    public class GetAppointmentsByExpertQueryHandler(
        IAppointmentRepository appointmentRepository)
        : IRequestHandler<GetAppointmentsByExpertQuery, BaseResponse<IEnumerable<AppointmentResponseForExpertDto>>>
    {
        public async Task<BaseResponse<IEnumerable<AppointmentResponseForExpertDto>>> Handle(GetAppointmentsByExpertQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<AppointmentResponseForExpertDto>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = []
            };

            try
            {
                // Lấy danh sách lịch hẹn từ repository dựa trên expertProfileId
                var appointments = await appointmentRepository.GetByExpertProfileIdAsync(request.ExpertProfileId);

                // Map dữ liệu từ Appointment sang AppointmentResponseForExpertDto
                var result = appointments.Select(a => new AppointmentResponseForExpertDto
                {
                    UserId = a.UserId,
                    Name = a.UserEmail,
                    AppointmentDate = a.AppointmentDate.ToString("yyyy-MM-dd"),
                    TimeRange = $"{a.StartTime:HH:mm} - {a.EndTime:HH:mm}",
                    MeetLink = a.MeetLink ?? "",
                    Tag = MapTag(a.Status, a.AppointmentDate, a.StartTime)
                });

                response.Success = true;
                response.Data = result;
                response.StatusCode = 200;
                response.Message = "Lấy danh sách lịch hẹn của chuyên gia thành công.";
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
