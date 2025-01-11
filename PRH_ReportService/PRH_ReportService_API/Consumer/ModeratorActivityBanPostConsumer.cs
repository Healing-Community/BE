using Application.Commands_Queries.Commnads.ModeratorActivity.PostReportActivity;
using Domain.Constants.AMQPMessage.Report;
using Domain.Entities.ModeratorActivity;
using MassTransit;
using MediatR;

namespace PRH_ReportService_API.Consumer;

public class ModeratorActivityBanPostConsumer(ISender sender) : IConsumer<BanPostMessage>
{
    public async Task Consume(ConsumeContext<BanPostMessage> context)
    {
        var banPostMessage = context.Message;

        var command = new CreatePostReportActivityCommand(banPostMessage);

        await sender.Send(command);

        await Console.Out.WriteLineAsync("Post report activity created");
    }
}
