using Infrastructure.Context;
using Infrastructure.Interface;
using Infrastructure.Mapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<UserServiceDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("PostgresDb"));
        });
        // Add Redis
        string? redisConnectionString = configuration.GetConnectionString("Redis");
        var redis = ConnectionMultiplexer.Connect(redisConnectionString ?? throw new NullReferenceException());
        services.AddScoped<IRedisContext, RedisContext>();
        services.AddSingleton<IConnectionMultiplexer>(redis);
        // Add AutoMapper
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
    }
}