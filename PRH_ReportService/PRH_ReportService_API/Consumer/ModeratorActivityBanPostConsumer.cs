using Domain.Constants.AMQPMessage.Report;
using MassTransit;

namespace PRH_ReportService_API.Consumer;

public class ModeratorActivityBanPostConsumer : IConsumer<BanPostMessage>
{
    public Task Consume(ConsumeContext<BanPostMessage> context)
    {
        throw new NotImplementedException();
    }
}
