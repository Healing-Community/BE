using Infrastructure.Context;
using Infrastructure.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<HFDBPostserviceContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("PostgresDb"));
        });
        // Add AutoMapper
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
    }
}