using Application.Commands_Queries.Commnads.Report;
using Domain.Constants.AMQPMessage;
using MassTransit;
using MediatR;
using System.Text.Json;

namespace PRH_ReportService_API.Consummer
{
    public class PostReportConsumer(ISender sender) : IConsumer<PostReportMessage>
    {
        public async Task Consume(ConsumeContext<PostReportMessage> context)
        {
            var response = await sender.Send(new CreatePostReportCommand(context.Message));
            await Console.Out.WriteLineAsync(JsonSerializer.Serialize(response));
        }
    }
}
