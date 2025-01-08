using Application.Interfaces;
using Application.Interfaces.AMQP;
using Microsoft.Extensions.DependencyInjection;
using Persistence.AMQP;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace Persistence;

public static class DependencyInjection
{
    public static void AddPersistenceDependencies(this IServiceCollection services)
    {
        #region MongoDB

        services.AddSingleton<IMongoClient>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var connectionString = config.GetConnectionString("MongoDb");
            return new MongoClient(connectionString);
        });

        services.AddScoped(typeof(IMongoRepository<>), typeof(MongoGenericMongoRepository<>));
        #endregion
        services.AddScoped<IMessagePublisher, MessagePublisher>();
    }
}