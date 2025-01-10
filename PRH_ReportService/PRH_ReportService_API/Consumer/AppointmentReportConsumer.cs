using System.Text.Json;
using Application.Commands_Queries.Commnads.Report.Appointment;
using Domain.Constants.AMQPMessage.Report;
using MassTransit;
using MediatR;

namespace PRH_ReportService_API.Consumer;

public class AppointmentReportConsumer(ISender sender) : IConsumer<ReportAppointmentMessage>
{
    public async Task Consume(ConsumeContext<ReportAppointmentMessage> context)
    {
        var response = await sender.Send(new CreateAppointmentReportCommand(context.Message));
        await Console.Out.WriteLineAsync(JsonSerializer.Serialize(response));
    }
}
