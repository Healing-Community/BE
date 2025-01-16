using Application.Commons.DTOs;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;

namespace Application.Queries.GetAppointmentsByUser
{
    public class GetAppointmentsByUserQueryHandler(
        IAppointmentRepository appointmentRepository,
        IExpertAvailabilityRepository expertAvailabilityRepository)
        : IRequestHandler<GetAppointmentsByUserQuery, BaseResponse<IEnumerable<AppointmentResponseForUserDto>>>
    {
        public async Task<BaseResponse<IEnumerable<AppointmentResponseForUserDto>>> Handle(GetAppointmentsByUserQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<AppointmentResponseForUserDto>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                // Lấy danh sách lịch hẹn từ repository dựa trên userId
                var appointments = await appointmentRepository.GetByUserIdAsync(request.UserId);

                // Tìm nạp ExpertAvailability cho mỗi cuộc hẹn để nhận Amount
                var result = new List<AppointmentResponseForUserDto>();
                foreach (var appointment in appointments)
                {
                    var expertAvailability = await expertAvailabilityRepository.GetByIdAsync(appointment.ExpertAvailabilityId);
                    if (expertAvailability != null)
                    {
                        result.Add(new AppointmentResponseForUserDto
                        {
                            AppointmentId = appointment.AppointmentId,
                            ExpertId = appointment.ExpertProfileId,
                            Amount = expertAvailability.Amount,
                            Name = appointment.ExpertProfile?.Fullname ?? "Không xác định",
                            AppointmentDate = appointment.AppointmentDate.ToString("yyyy-MM-dd"),
                            TimeRange = $"{appointment.StartTime:HH:mm} - {appointment.EndTime:HH:mm}",
                            MeetLink = appointment.MeetLink ?? "",
                            Tag = MapTag(appointment.Status, appointment.AppointmentDate, appointment.StartTime)
                        });
                    }
                }

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
                0 => "Chờ thanh toán",
                1 when appointmentDateTime > now => "Sắp diễn ra",
                2 => "Đã hủy",
                3 => "Đã hoàn thành",
                4 => "Hủy thanh toán",
                5 => "Đã hoàn tiền",
                _ => "Không xác định"
            };
        }
    }
}
