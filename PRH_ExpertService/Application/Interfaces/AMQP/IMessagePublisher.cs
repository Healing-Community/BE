using Domain.Constants;

namespace Application.Interfaces.AMQP
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message, QueueName queueName, CancellationToken cancellationToken);
    }
}
