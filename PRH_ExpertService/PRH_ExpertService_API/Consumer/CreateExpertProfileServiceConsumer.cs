using Application.Interfaces.Repository;
using Domain.Constants.AMQPMessage;
using MassTransit;

namespace PRH_ExpertService_API.Consumer
{
    public class CreateExpertProfileServiceConsumer (IExpertProfileRepository expertProfileRepository) : IConsumer<CreateExpertMessage>
    {
        public async Task Consume(ConsumeContext<CreateExpertMessage> context)
        {
            var message = context.Message;

            await expertProfileRepository.Create(new Domain.Entities.ExpertProfile
            {
                ExpertProfileId = message.UserId,
                UserId = message.UserId,
                Fullname = string.Empty,
                Email = string.Empty,
                Specialization = string.Empty,
                ExpertiseAreas = string.Empty,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                UpdatedAt = DateTime.UtcNow.AddHours(7)
            });
        }
    }
}
