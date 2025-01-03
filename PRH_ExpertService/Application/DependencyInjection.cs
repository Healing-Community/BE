using System.Reflection;
using Application.Interfaces.Services;
using Application.Jobs;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
    {
        services.AddScoped<IFirebaseStorageService, FirebaseStorageService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<AppointmentStatusJob>();

        services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()); });

        services.AddHttpContextAccessor();

        return services;
    }
}