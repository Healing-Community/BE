using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using CleanArchitecture.Persistence.AMQP;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;


namespace Persistence;

public static class DependencyInjection
{
    public static void AddPersistenceDependencies(this IServiceCollection services)
    {
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IMessagePublisher, MessagePublisher>();

        //#region MongoDB

        //services.AddSingleton<IMongoClient>(sp =>
        //{
        //    var config = sp.GetRequiredService<IConfiguration>();
        //    var connectionString = config.GetConnectionString("MongoDb");
        //    return new MongoClient(connectionString);
        //});

        //services.AddScoped(typeof(IMongoRepository<>), typeof(MongoGenericMongoRepository<>));
        //#endregion

    }
}