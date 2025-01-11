using System;
using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Queries.Commnads.Report.Appointment.Update;

public class UpdateAppointmetReportStatusCommandHandler(IMongoRepository<AppointmentReport> repository) : IRequestHandler<UpdateAppointmetReportStatusCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(UpdateAppointmetReportStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var appointmentReport = await repository.GetByPropertyAsync(x => x.Id == request.AppointmentId);
            if (appointmentReport == null)
            {
                return BaseResponse<string>.NotFound("Appointment report not found");
            }

            appointmentReport.IsApprove = request.IsApprove;
            appointmentReport.UpdatedAt = DateTime.UtcNow + TimeSpan.FromHours(7);

            await repository.UpdateAsync(appointmentReport.Id, appointmentReport);

            return BaseResponse<string>.SuccessReturn(appointmentReport.Id);
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}
