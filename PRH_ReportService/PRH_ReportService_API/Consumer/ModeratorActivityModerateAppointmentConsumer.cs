using System;
using Application.Commands_Queries.Commnads.ModeratorActivity.AppointmentReportActivity;
using Domain.Constants.AMQPMessage.Report;
using MassTransit;
using MediatR;

namespace PRH_ReportService_API.Consumer;

public class ModeratorActivityModerateAppointmentConsumer(ISender sender) : IConsumer<ModerateAppointmentMessage>
{
    public async Task Consume(ConsumeContext<ModerateAppointmentMessage> context)
    {

        var message = context.Message;
        var command = new CreateAppointmentReportActivityCommand(message);
        await sender.Send(command);
        await Console.Out.WriteLineAsync("Moderate Appointment Report Activity");

    }
}
