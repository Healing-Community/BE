using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;
using NUlid;

namespace Application.Commands.CancelAppointment
{
    public class CancelAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IExpertAvailabilityRepository availabilityRepository) : IRequestHandler<CancelAppointmentCommand, BaseResponse<bool>>
    {
        public async Task<BaseResponse<bool>> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse<bool>
            {
                Id = Ulid.NewUlid().ToString(),
                Timestamp = DateTime.UtcNow.AddHours(7),
                Errors = new List<string>()
            };

            try
            {
                var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);

                if (appointment.Status == 4 || appointment.Status == 5)
                {
                    response.Success = false;
                    response.Message = "Không thể hủy cuộc hẹn đã hoàn thành hoặc đã bị hủy.";
                    response.StatusCode = 400;
                    return response;
                }

                appointment.Status = 4; // Cancelled
                await appointmentRepository.Update(appointment.AppointmentId, appointment);

                var availability = await availabilityRepository.GetByIdAsync(appointment.ExpertAvailabilityId);
                if (availability != null)
                {
                    availability.Status = 0; // Available
                    await availabilityRepository.Update(availability.ExpertAvailabilityId, availability);
                }

                response.Success = true;
                response.Data = true;
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.StatusCode = 500;
            }

            return response;
        }
    }
}