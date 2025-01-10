using System.Reflection;
using Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
    {
        services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()); });
        services.AddHttpContextAccessor();
        services.AddScoped<IGrpcHelper,GrpcHelper>();
        return services;
    }
}