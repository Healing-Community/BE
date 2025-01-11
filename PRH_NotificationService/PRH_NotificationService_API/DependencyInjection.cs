using System.Text;
using Domain.Constants;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PRH_NotificationService_API.Consumer;

namespace PRH_NotificationService_API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentationDependencies(this IServiceCollection services,
        IConfiguration configuration)
    {
        #region Base-configuration

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddRouting(options => { options.LowercaseUrls = true; });

        #endregion

        #region Swagger

        var generalSettings = configuration.GetSection("GeneralSettings");
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = generalSettings["ApiName"],
                    Version = generalSettings["ApiVersion"],
                    Description = generalSettings["ApiDescription"]
                });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please insert JWT with Bearer into field",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        #endregion

        #region Authorization-Authentication

        var jwtSettings = configuration.GetSection("JwtSettings");
        services.AddAuthorization();
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(jwtSettings["Secret"] ?? throw new InvalidOperationException())),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true
            };
        });

        #endregion

        #region AMQP
        var rabbitMq = configuration.GetSection("RabbitMq");

        services.AddMassTransit(x =>
        {
            x.AddConsumer<PostServiceConsumer>();
            x.AddConsumer<ReactionServiceConsumer>();
            x.AddConsumer<CommentServiceConsumer>();
            x.AddConsumer<ReportServiceConsumer>();
            x.AddConsumer<MailConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(rabbitMq["Host"] ?? throw new NullReferenceException()), h =>
                {
                    h.Username(rabbitMq["Username"] ?? throw new NullReferenceException());
                    h.Password(rabbitMq["Password"] ?? throw new NullReferenceException());
                });

                // Register PostServiceConsumer
                cfg.ReceiveEndpoint(QueueName.PostQueue.ToString(), c =>
                {
                    c.ConfigureConsumer<PostServiceConsumer>(context);
                });

                // Register ReactionServiceConsumer
                cfg.ReceiveEndpoint(QueueName.ReactionQueue.ToString(), c =>
                {
                    c.ConfigureConsumer<ReactionServiceConsumer>(context);
                });

                // Register CommentServiceConsumer
                cfg.ReceiveEndpoint(QueueName.CommentQueue.ToString(), c =>
                {
                    c.ConfigureConsumer<CommentServiceConsumer>(context);
                });

                // Register ReportServiceConsumer
                cfg.ReceiveEndpoint(QueueName.ReportQueue.ToString(), c =>
                {
                    c.ConfigureConsumer<ReportServiceConsumer>(context);
                });

                // Register MailComsumer
                cfg.ReceiveEndpoint(QueueName.MailQueue.ToString(), c =>
                {
                    c.ConfigureConsumer<MailConsumer>(context);
                });

                // Configure Retry
                cfg.UseMessageRetry(retryConfig =>
                {
                    retryConfig.Interval(5, TimeSpan.FromSeconds(5)); // Retry 5 times, every 5 seconds
                });



                // Configure CircuitBreaker
                cfg.UseCircuitBreaker(cbConfig =>
                {
                    cbConfig.TrackingPeriod = TimeSpan.FromMinutes(1);
                    cbConfig.ActiveThreshold = 5;
                    cbConfig.ResetInterval = TimeSpan.FromMinutes(5);
                });
            });
        });

        // Add MassTransit hosted service
        services.AddHostedService<MassTransitHostedService>();

        #endregion

        #region CORS

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        #endregion

        //#region HealthCheck

        //// Retrieve connection strings and settings from configuration
        //string postgresConnectionString = configuration.GetConnectionString("PostgresDb") ?? throw new NullReferenceException();
        //services.AddHealthChecks()
        //        .AddCheck("Self", () => HealthCheckResult.Healthy(), tags: ["liveness"])
        //        .AddNpgSql(
        //            configuration.GetConnectionString("PostgresDb") ?? throw new NullReferenceException(),
        //            name: "PostgresDb-check",
        //            tags: ["db", "postgres", "readiness"],
        //            healthQuery: "SELECT 1;",
        //            failureStatus: HealthStatus.Unhealthy
        //        )
        //        .AddRabbitMQ(
        //            rabbitConnectionString: rabbitMq["Host"] ?? throw new NullReferenceException(),
        //            name: "RabbitMq-check",
        //            tags: ["rabbitmq", "messaging", "readiness"],
        //            failureStatus: HealthStatus.Unhealthy
        //        );
        //#endregion

        return services;
    }
}