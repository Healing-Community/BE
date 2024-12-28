using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Commands.UpdateAvailability.UpdateAvailabilityStatus;

public class UpdateAvailabilityStatusCommandHandler(IAppointmentRepository appointmentRepository, IExpertAvailabilityRepository expertAvailabilityRepository) : IRequestHandler<UpdateAvailabilityStatusCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(UpdateAvailabilityStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);
            if (appointment == null)
            {
                return BaseResponse<string>.NotFound("không tìm thấy thông tin lịch hẹn");
            }
            var expertAvailability = await expertAvailabilityRepository.GetByIdAsync(appointment.ExpertAvailabilityId);
            if (expertAvailability == null)
            {
                return BaseResponse<string>.NotFound("không tìm thấy thông tin lịch hẹn");
            }
            expertAvailability.Status = request.Status;
            await expertAvailabilityRepository.Update(expertAvailability.ExpertAvailabilityId, expertAvailability);
            return BaseResponse<string>.SuccessReturn("Cập nhật trạng thái lịch hẹn thành công");
        }
        catch (Exception e) 
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}