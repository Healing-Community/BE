using System.Text.Json;
using Application.Commands_Queries.Commnads.Report.Comment;
using Domain.Constants.AMQPMessage;
using Domain.Entities;
using MassTransit;
using MediatR;

namespace PRH_ReportService_API.Consumer;

public class CommentReportConsumer(ISender sender,IMongoRepository<CommentReport> commentReportRepository) : IConsumer<CommentReportMessage>
{
    public async Task Consume(ConsumeContext<CommentReportMessage> context)
    {
        var commentInDb = await commentReportRepository.GetByPropertyAsync(c => c.UserId == context.Message.UserId && c.CommentId == context.Message.CommentId);
        if (commentInDb != null)
        {
            commentInDb.ReportTypeEnum = context.Message.ReportTypeEnum;
            await commentReportRepository.UpdateAsync(commentInDb.Id, commentInDb);
            await Console.Out.WriteLineAsync("Already reported this comment - updated the report type");
            return;
        }
        var response = await sender.Send(new CreateCommentReportCommand(context.Message));
        await Console.Out.WriteLineAsync(JsonSerializer.Serialize(response));
    }
}
