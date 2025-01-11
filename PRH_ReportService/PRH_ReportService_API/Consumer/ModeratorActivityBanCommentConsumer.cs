using System;
using Domain.Constants.AMQPMessage.Report;
using MassTransit;

namespace PRH_ReportService_API.Consumer;

public class ModeratorActivityBanCommentConsumer : IConsumer<BanCommentMessage>
{
    public Task Consume(ConsumeContext<BanCommentMessage> context)
    {
        throw new NotImplementedException();
    }
}
