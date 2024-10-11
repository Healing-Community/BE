using Application.Commons.Request;
using MassTransit;

namespace PRH_NotificationService_API.Consumer
{
    public class PostServiceConsumer : IConsumer<PostingRequestCreatedMessage>
    {
        public async Task Consume(ConsumeContext<PostingRequestCreatedMessage> context)
        {
            var postingRequest = context.Message;

            await Console.Out.WriteLineAsync($"Received Posting Request: {postingRequest.Tittle}");
        }
    }
}
