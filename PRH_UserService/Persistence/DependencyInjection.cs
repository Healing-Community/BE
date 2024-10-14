using Application.Interfaces.AMQP;
using Application.Interfaces.Repository;
using Microsoft.Extensions.DependencyInjection;
using Persistence.AMQP;
using Persistence.Repositories;

namespace Persistence;

public static class DependencyInjection
{
    public static void AddPersistenceDependencies(this IServiceCollection services)
    {
        services.AddScoped<IMessagePublisher, MessagePublisher>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IJwtTokenRepository, JwtTokenRepository>();
        services.AddScoped<IEmailRepository, EmailRepository>();
        services.AddScoped<ITokenRepository, TokenRepository>();
    }
}