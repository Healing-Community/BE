using Application.Interfaces;
using Application.Interfaces.AMQP;
using Microsoft.Extensions.DependencyInjection;
using Persistence.AMQP;
using Persistence.Repositories;

namespace Persistence;

public static class DependencyInjection
{
    public static void AddPersistenceDependencies(this IServiceCollection services)
    {
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IMessagePublisher, MessagePublisher>();
    }
}