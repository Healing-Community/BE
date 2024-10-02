using Application.Interfaces.Repository;
using Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;


namespace Persistence;

public static class DependencyInjection
{
    public static void AddPersistenceDependencies(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IRoleRepository, RoleRepository>();

        services.AddScoped<IJwtTokenRepository, JwtTokenRepository>();


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