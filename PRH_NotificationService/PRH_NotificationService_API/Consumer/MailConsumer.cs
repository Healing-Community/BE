using Application.Interfaces.Repository;
using Domain.Constants.AMQPMessage;
using MassTransit;

namespace PRH_NotificationService_API.Consumer;

public class MailConsumer(IEmailRepository emailRepository) : IConsumer<SendMailMessage>
{
    public async Task Consume(ConsumeContext<SendMailMessage> context)
    {
        var message = context.Message;
        var sendMailMessage = new SendMailMessage
        {
            To = message.To,
            Subject = message.Subject,
            Body = message.Body
        };
        await emailRepository.SendEmailAsync(sendMailMessage.To, sendMailMessage.Subject, sendMailMessage.Body);
        // Email sent successfully
    }
}
