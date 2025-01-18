namespace Domain.Constants.AMQPMessage;

public class CreateExpertMessage
{
    public required string UserId { get; set; }
    public required string UserName { get; set; }
    public required string UserEmail { get; set; }
}