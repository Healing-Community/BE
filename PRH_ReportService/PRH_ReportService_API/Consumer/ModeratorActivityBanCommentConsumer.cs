using System;
using Application.Commands_Queries.Commnads.ModeratorActivity.CommentReportActivity;
using Domain.Constants.AMQPMessage.Report;
using MassTransit;
using MediatR;

namespace PRH_ReportService_API.Consumer;

public class ModeratorActivityBanCommentConsumer(ISender sender) : IConsumer<BanCommentMessage>
{
    public async Task Consume(ConsumeContext<BanCommentMessage> context)
    {
        var message = context.Message;
        var command = new CreateCommentReportActivityCommand(message);
        await sender.Send(command);
        await Console.Out.WriteLineAsync("Comment report activity created");
    }
}
