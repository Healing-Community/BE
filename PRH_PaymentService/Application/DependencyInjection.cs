using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Net.payOS;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services, IConfiguration configuration)
    {

        


        services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()); });

        var payOsConfig = configuration.GetSection("Environment");
        PayOS payOS = new PayOS(
            payOsConfig["PAYOS_CLIENT_ID"] ?? throw new Exception("Client ID is not configured"),
            payOsConfig["PAYOS_API_KEY"] ?? throw new Exception("API key is not configured"),
            payOsConfig["PAYOS_CHECKSUM_KEY"] ?? throw new Exception("Checksum key is not configured")
        );

        services.AddSingleton(payOS);

        services.AddHttpContextAccessor();

        return services;
    }
}
