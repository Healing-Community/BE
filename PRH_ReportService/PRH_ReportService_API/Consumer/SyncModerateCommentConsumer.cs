using System;
using Application.Commands_Queries.Commnads.Report.Comment.Update;
using Domain.Constants.AMQPMessage.Report;
using MassTransit;
using MediatR;

namespace PRH_ReportService_API.Consumer;

public class SyncModerateCommentConsumer(ISender sender) : IConsumer<SyncBanCommentReportMessage>
{
    public async Task Consume(ConsumeContext<SyncBanCommentReportMessage> context)
    {
        var message = context.Message;
        await sender.Send(new UpdateCommentReportStatus(message.CommentId,message.IsApprove));
    }
}