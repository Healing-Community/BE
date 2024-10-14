using Application.Commands.Reports.CreateReport;
using Application.Commons.DTOs;
using Domain.Constants.AMQPMessage;
using MassTransit;
using MediatR;
using System.Text.Json;

namespace PRH_ReportService_API.Consummer
{
    public class ReportConsumer(ISender sender) : IConsumer<ReportMessage>
    {
        public async Task Consume(ConsumeContext<ReportMessage> context)
        {
            var message = context.Message;
            var response = await sender.Send(new CreateReportCommand(new ReportDto
            {
                UserId = message.UserId,
                CommentId = message.CommentId,
                ReportTypeId = message.ReportTypeId,
                Description = message.Description,
                ExpertId = message.ExpertId,
                PostId = message.PostId,
                TargetUserId = message.TargetUserId
            }));
            await Console.Out.WriteLineAsync(JsonSerializer.Serialize(response));
        }
    }
}
