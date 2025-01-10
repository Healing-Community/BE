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
        services.AddScoped<IExpertProfileRepository, ExpertProfileRepository>();
        services.AddScoped<ICertificateRepository, CertificateRepository>();
        services.AddScoped<ICertificateTypeRepository, CertificateTypeRepository>();
        services.AddScoped<IExpertAvailabilityRepository, ExpertAvailabilityRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IWorkExperienceRepository, WorkExperienceRepository>();

        services.AddScoped<IMessagePublisher, MessagePublisher>();

    }
}