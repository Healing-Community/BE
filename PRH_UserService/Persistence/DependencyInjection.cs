using Application.Interfaces.AMQP;
using Application.Interfaces.Redis;
using Application.Interfaces.Repository;
using Microsoft.Extensions.DependencyInjection;
using Persistence.AMQP;
using Persistence.Cache;
using Persistence.Repositories;

namespace Persistence;

public static class DependencyInjection
{
    public static void AddPersistenceDependencies(this IServiceCollection services)
    {
        // AMQP
        services.AddScoped<IMessagePublisher, MessagePublisher>();
        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IEmailRepository, EmailRepository>();
        services.AddScoped<ITokenRepository, TokenRepository>();
        services.AddScoped<ISocialLinkRepository, SocialLinkRepository>();
        // Cache repositories
        services.AddScoped<IOtpCache, OtpCache>();
    }
}