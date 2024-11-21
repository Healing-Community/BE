using Infrastructure.Mapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add AutoMapper
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        return services;
    }
}