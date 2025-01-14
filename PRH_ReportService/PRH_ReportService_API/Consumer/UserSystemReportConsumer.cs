using Application.Commands_Queries.Commnads.Report.SystemReport;
using Domain.Constants.AMQPMessage;
using MassTransit;
using MediatR;

namespace PRH_ReportService_API.Consumer;

public class UserSystemReportConsumer(ISender sender) : IConsumer<UserReportSystemMessage>
{
    public async Task Consume(ConsumeContext<UserReportSystemMessage> context)
    {
        var message = context.Message;
        await sender.Send(new CreateUserReportSystemCommand(message));
        await Console.Out.WriteLineAsync($"UserSystemReportConsumer: {message}");
    }
}
