namespace Domain.Constants.AMQPMessage;

public class SendMailMessage
{
    public required string To { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; }
}
