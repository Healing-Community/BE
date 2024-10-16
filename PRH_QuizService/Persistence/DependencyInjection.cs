using Application.Interfaces;
using Application.Interfaces.AMQP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Persistence.AMQP;
namespace Persistence;

public static class DependencyInjection
{
    public static void AddPersistenceDependencies(this IServiceCollection services)
    {
        services.AddScoped<IMessagePublisher, MessagePublisher>();


        #region MongoDB

        services.AddSingleton<IMongoClient>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var connectionString = config.GetConnectionString("MongoDb");
            return new MongoClient(connectionString);
        });

        services.AddScoped(typeof(IMongoRepository<>), typeof(MongoGenericMongoRepository<>));
        #endregion
    }
}