using Application.Commands_Queries.Commnads.Report.Appointment.Update;
using Domain.Constants.AMQPMessage.Report;
using MassTransit;
using MediatR;

namespace PRH_ReportService_API.Consumer;

public class SyncModerateAppointmentConsumer(ISender sender) : IConsumer<SyncModerateAppointmentMessage>
{
    public async Task Consume(ConsumeContext<SyncModerateAppointmentMessage> context)
    {
        var message = context.Message;
        await sender.Send(new UpdateAppointmetReportStatusCommand(message.AppointmentId,message.IsApprove));
    }
}
