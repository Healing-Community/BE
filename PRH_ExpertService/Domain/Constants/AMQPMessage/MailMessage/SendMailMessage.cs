using System;

namespace Domain.Constants.AMQPMessage.MailMessage;

public class SendMailMessage
{
    public required string To { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; }
}
