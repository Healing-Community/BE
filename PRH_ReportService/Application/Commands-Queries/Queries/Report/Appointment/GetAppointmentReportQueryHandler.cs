using System;
using Application.Commons;
using Domain.Entities;
using MediatR;

namespace Application.Commands_Queries.Queries.Report.Appointment;

public class GetAppointmentReportQueryHandler(IMongoRepository<AppointmentReport> appointmentReportRepository) : IRequestHandler<GetAppointmentReportQuery, BaseResponse<IEnumerable<AppointmentReport>>>
{
    public async Task<BaseResponse<IEnumerable<AppointmentReport>>> Handle(GetAppointmentReportQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var appointmentReports = await appointmentReportRepository.GetsAsync();
            return BaseResponse<IEnumerable<AppointmentReport>>.SuccessReturn(appointmentReports);
        }
        catch (Exception e)
        {
            return BaseResponse<IEnumerable<AppointmentReport>>.InternalServerError(e.Message);
            throw;
        }
    }
}
