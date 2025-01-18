using Application.Commands_Queries.Commnads.Report;
using Domain.Constants.AMQPMessage;
using Domain.Entities;
using MassTransit;
using MediatR;
using System.Text.Json;

namespace PRH_ReportService_API.Consummer
{
    public class PostReportConsumer(ISender sender, IMongoRepository<PostReport> postRepository) : IConsumer<PostReportMessage>
    {
        public async Task Consume(ConsumeContext<PostReportMessage> context)
        {
            var postInDb = await postRepository.GetByPropertyAsync(p => p.UserId == context.Message.UserId && p.PostId == context.Message.PostId);
            if (postInDb != null)
            {
                postInDb.ReportTypeEnum = context.Message.ReportTypeEnum;
                await postRepository.UpdateAsync(postInDb.Id, postInDb);
                await Console.Out.WriteLineAsync("Already reported this post");
                return;
            }
            var response = await sender.Send(new CreatePostReportCommand(context.Message));
            await Console.Out.WriteLineAsync(JsonSerializer.Serialize(response));
        }
    }
}
