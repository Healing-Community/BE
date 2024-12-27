using Application.Commons;
using Application.Interfaces.Repository;
using MediatR;

namespace Application.Queries.GetExpertAvailbilityByAppointmentId;

public class GetExpertAvailbilityByAppointmentIdQueryHandler(IAppointmentRepository appointmentRepository,IExpertAvailabilityRepository expertAvailabilityRepository) : IRequestHandler<GetExpertAvailbilityByAppointmentIdQuery, BaseResponse<ExpertAvailabilityResponseDto>>
{
    public async Task<BaseResponse<ExpertAvailabilityResponseDto>> Handle(GetExpertAvailbilityByAppointmentIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId);
            if (appointment == null)
            {
                return BaseResponse<ExpertAvailabilityResponseDto>.NotFound("Không tìm thấy lịch hẹn");
            }
            var expertAvailability = await expertAvailabilityRepository.GetByIdAsync(appointment.ExpertAvailabilityId);
            if (expertAvailability == null)
            {
                return BaseResponse<ExpertAvailabilityResponseDto>.NotFound("Không tìm thấy thông tin ca làm việc của chuyên gia");
            }
            ExpertAvailabilityResponseDto expertAvailabilityRes = new ExpertAvailabilityResponseDto
            {
                Amount = expertAvailability.Amount,
            };

            if (expertAvailability == null)
            {
                return BaseResponse<ExpertAvailabilityResponseDto>.NotFound("Không tìm thấy thông tin ca làm việc của chuyên gia");
            }
            return BaseResponse<ExpertAvailabilityResponseDto>.SuccessReturn(expertAvailabilityRes);
        }
        catch (Exception e)
        {
            return BaseResponse<ExpertAvailabilityResponseDto>.InternalServerError(e.Message);
            throw;
        }
    }
}
