using Application.Interfaces.AMQP;
using Microsoft.Extensions.DependencyInjection;
using Persistence.AMQP;

namespace Persistence;

public static class DependencyInjection
{
    public static void AddPersistenceDependencies(this IServiceCollection services)
    {
        // AMQP
        services.AddScoped<IMessagePublisher, MessagePublisher>();
        // Repositories

        // Cache repositories
    }
}
