using System;
using Domain.Constants.AMQPMessage.Report;
using MassTransit;

namespace PRH_ReportService_API.Consumer;

public class ModeratorActivityModerateAppointmentConsumer : IConsumer<ModerateAppointmentMessage>
{
    public Task Consume(ConsumeContext<ModerateAppointmentMessage> context)
    {
        throw new NotImplementedException();
    }
}
