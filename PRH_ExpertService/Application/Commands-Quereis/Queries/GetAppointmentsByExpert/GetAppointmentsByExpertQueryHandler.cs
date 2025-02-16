﻿using Application.Commons.DTOs;
using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;

namespace Application.Queries.GetAppointmentsByExpert
{
    public class GetAppointmentsByExpertQueryHandler(
        IAppointmentRepository appointmentRepository,
        IExpertAvailabilityRepository expertAvailabilityRepository)
        : IRequestHandler<GetAppointmentsByExpertQuery, BaseResponse<IEnumerable<AppointmentResponseForExpertDto>>>
    {
        public async Task<BaseResponse<IEnumerable<AppointmentResponseForExpertDto>>> Handle(GetAppointmentsByExpertQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<AppointmentResponseForExpertDto>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                // Lấy danh sách lịch hẹn từ repository dựa trên expertProfileId
                var appointments = await appointmentRepository.GetByExpertProfileIdAsync(request.ExpertProfileId);

                // Tìm nạp ExpertAvailability cho mỗi cuộc hẹn để nhận Amount
                var result = new List<AppointmentResponseForExpertDto>();
                foreach (var appointment in appointments)
                {
                    var expertAvailability = await expertAvailabilityRepository.GetByIdAsync(appointment.ExpertAvailabilityId);
                    if (expertAvailability != null)
                    {
                        result.Add(new AppointmentResponseForExpertDto
                        {
                            AppointmentId = appointment.AppointmentId,
                            UserId = appointment.UserId,
                            Name = appointment.UserEmail,
                            Amount = expertAvailability.Amount,
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
                0 => "Chờ thanh toán",
                1 when appointmentDateTime > now => "Sắp diễn ra",
                2 => "Đã hủy",
                3 => "Đã hoàn thành",
                4 => "Hủy thanh toán",
                5 => "Hoàn tiền cho người dùng",
                6 => "Chờ báo cáo",
                7 => "Duyệt báo cáo người dùng thành công",
                8 => "Duyệt báo cáo người dùng thất bại",
                9 => "Chuyển tiền cho người dùng",
                10 => "Chuyển tiền cho chuyên gia",
                _ => "Không xác định"
            };
        }
    }
}
