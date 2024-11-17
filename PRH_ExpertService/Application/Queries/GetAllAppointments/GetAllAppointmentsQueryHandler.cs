using Application.Commons;
using Application.Interfaces.Repository;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Queries.GetAllAppointments
{
    public class GetAllAppointmentsQueryHandler(IAppointmentRepository appointmentRepository)
        : IRequestHandler<GetAllAppointmentsQuery, BaseResponse<IEnumerable<Appointment>>>
    {
        public async Task<BaseResponse<IEnumerable<Appointment>>> Handle(GetAllAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<IEnumerable<Appointment>>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = []
            };

            try
            {
                var appointments = await appointmentRepository.GetsAsync();

                response.Success = true;
                response.Data = appointments;
                response.StatusCode = 200;
                response.Message = "Lấy danh sách cuộc hẹn thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Đã xảy ra lỗi khi lấy danh sách cuộc hẹn.";
                response.StatusCode = 500;
                response.Errors.Add($"Chi tiết lỗi: {ex.Message}");
            }

            return response;
        }
    }
}
