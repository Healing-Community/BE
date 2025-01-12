using Application.Commands_Queries.Commnads.Report.Post.Update;
using Domain.Constants.AMQPMessage.Report;
using MassTransit;
using MediatR;

namespace PRH_ReportService_API.Consumer;

public class SyncModeratePostComsumer(ISender sender) : IConsumer<SyncBanPostReportMessage>
{
    public async Task Consume(ConsumeContext<SyncBanPostReportMessage> context)
    {
        var message = context.Message;
        await sender.Send(new UpdatePostReportStatusCommand(message.PostId,message.IsApprove));
        await Console.Out.WriteLineAsync("Moderator report updated successfully");
    }
}
