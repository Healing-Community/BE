using Domain.Constants;
using Domain.Constants.AMQPMessage;

namespace Application.Interfaces.AMQP
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, QueueName queueName, CancellationToken cancellationToken);
    }
}
