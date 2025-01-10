using Application.Commons;
using Domain.Entities;
using MediatR;
using NUlid;

namespace Application.Commands_Queries.Commnads.Report.Appointment;

public class CreateAppointmentReportCommandHandler(IMongoRepository<AppointmentReport> appointmentReportRepository) : IRequestHandler<CreateAppointmentReportCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(CreateAppointmentReportCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var appointmentReport = new AppointmentReport
            {
                Id = Ulid.NewUlid().ToString(),
                ReportDescription = request.ReportAppointmentMessage.ReportDescription,
                AppoinmtentDate = request.ReportAppointmentMessage.AppoinmtentDate,
                EndTime = request.ReportAppointmentMessage.EndTime,
                ExpertEmail = request.ReportAppointmentMessage.ExpertEmail,
                ExpertName = request.ReportAppointmentMessage.ExpertName,
                StartTime = request.ReportAppointmentMessage.StartTime,
                UserId = request.ReportAppointmentMessage.UserId,
                AppointmentId = request.ReportAppointmentMessage.AppointmentId,

                UserEmail = request.ReportAppointmentMessage.UserEmail,
                UserName = request.ReportAppointmentMessage.UserName,

                CreatedAt = DateTime.UtcNow + TimeSpan.FromHours(7),
                UpdatedAt = DateTime.UtcNow + TimeSpan.FromHours(7)
            };

            await appointmentReportRepository.Create(appointmentReport);

            return BaseResponse<string>.SuccessReturn(appointmentReport.Id);
        }
        catch (Exception e)
        {
            return BaseResponse<string>.InternalServerError(e.Message);
            throw;
        }
    }
}
