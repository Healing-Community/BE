using System;
using System.Text.Json;
using Application.Commands_Queries.Commnads.Report.Comment;
using Domain.Constants.AMQPMessage;
using MassTransit;
using MediatR;

namespace PRH_ReportService_API.Consumer;

public class CommentReportConsumer(ISender sender) : IConsumer<CommentReportMessage>
{
    public async Task Consume(ConsumeContext<CommentReportMessage> context)
    {
        var response = await sender.Send(new CreateCommentReportCommand(context.Message));
        await Console.Out.WriteLineAsync(JsonSerializer.Serialize(response));
    }
}
