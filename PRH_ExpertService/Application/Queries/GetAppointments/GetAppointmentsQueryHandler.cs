using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Queries.GetAppointments
{
    public class GetAppointmentsQueryHandler(IAppointmentRepository appointmentRepository) : IRequestHandler<GetAppointmentsQuery, BaseResponse<IEnumerable<Appointment>>>
    {
        public async Task<BaseResponse<IEnumerable<Appointment>>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<Appointment>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow,
                Errors = []
            };

            try
            {
                var appointments = await appointmentRepository.GetByExpertProfileIdAsync(request.ExpertProfileId);

                response.Success = true;
                response.Data = appointments;
                response.StatusCode = 200;
                response.Message = "Lấy danh sách lịch hẹn thành công.";
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
    }
}
