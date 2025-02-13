﻿using System.Text;
using Domain.Constants;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PRH_ReportService_API.Consumer;
using PRH_ReportService_API.Consummer;

namespace PRH_ReportService_API;

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

        #region AMQP
        var rabbitMq = configuration.GetSection("RabbitMq");
        services.AddMassTransit(x =>
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<PostReportConsumer>();
                x.AddConsumer<CommentReportConsumer>();
                x.AddConsumer<SyncModerateAppointmentConsumer>();
                x.AddConsumer<SyncModeratePostComsumer>();
                x.AddConsumer<SyncModerateCommentConsumer>();
                x.AddConsumer<ModeratorActivityBanCommentConsumer>();
                x.AddConsumer<ModeratorActivityBanPostConsumer>();
                x.AddConsumer<ModeratorActivityModerateAppointmentConsumer>();
                x.AddConsumer<AppointmentReportConsumer>();
                x.AddConsumer<UserSystemReportConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri(rabbitMq["Host"] ?? throw new NullReferenceException()), h =>
                    {
                        h.Username(rabbitMq["Username"] ?? throw new NullReferenceException());
                        h.Password(rabbitMq["Password"] ?? throw new NullReferenceException());
                    });

                    // Đăng ký consumer
                    cfg.ReceiveEndpoint(QueueName.PostReportQueue.ToString(), c =>
                    {
                        c.ConfigureConsumer<PostReportConsumer>(context);
                    });
                    cfg.ReceiveEndpoint(QueueName.CommentReportQueue.ToString(), c =>
                    {
                        c.ConfigureConsumer<CommentReportConsumer>(context);
                    });
                    cfg.ReceiveEndpoint(QueueName.SyncAppointmentReportQueue.ToString(), c =>
                    {
                        c.ConfigureConsumer<SyncModerateAppointmentConsumer>(context);
                    });
                    cfg.ReceiveEndpoint(QueueName.SyncPostReportQueue.ToString(), c =>
                    {
                        c.ConfigureConsumer<SyncModeratePostComsumer>(context);
                    });
                    cfg.ReceiveEndpoint(QueueName.SyncCommentReportQueue.ToString(), c =>
                    {
                        c.ConfigureConsumer<SyncModerateCommentConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(QueueName.AppointmentModerateQueue.ToString(), c =>
                    {
                        c.ConfigureConsumer<ModeratorActivityModerateAppointmentConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(QueueName.BanPostQueue.ToString(), c =>
                    {
                        c.ConfigureConsumer<ModeratorActivityBanPostConsumer>(context);
                    });

                    cfg.ReceiveEndpoint(QueueName.BanCommentQueue.ToString(), c =>
                    {
                        c.ConfigureConsumer<ModeratorActivityBanCommentConsumer>(context);
                    });
                    cfg.ReceiveEndpoint(QueueName.AppointmentReportQueue.ToString(), c =>
                    {
                        c.ConfigureConsumer<AppointmentReportConsumer>(context);
                    });
                    cfg.ReceiveEndpoint(QueueName.UserReportSystemQueue.ToString(), c =>
                    {
                        c.ConfigureConsumer<UserSystemReportConsumer>(context);
                    });

                    // Thiết lập Retry
                    cfg.UseMessageRetry(retryConfig =>
                    {
                        retryConfig.Interval(5, TimeSpan.FromSeconds(5)); // Thử lại 5 lần, mỗi lần cách nhau 5 giây
                    });

                    // Tùy chọn khác như Timeout, CircuitBreaker nếu cần
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
        });
        # endregion
        
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

        #region HealthCheck

        // Retrieve connection strings and settings from configuration
        string postgresConnectionString = configuration.GetConnectionString("PostgresDb") ?? throw new NullReferenceException();
        services.AddHealthChecks()
                .AddCheck("Self", () => HealthCheckResult.Healthy(), tags: ["liveness"])
                .AddNpgSql(
                    configuration.GetConnectionString("PostgresDb") ?? throw new NullReferenceException(),
                    name: "PostgresDb-check",
                    tags: ["db", "postgres", "readiness"],
                    healthQuery: "SELECT 1;",
                    failureStatus: HealthStatus.Unhealthy
                )
                .AddRabbitMQ(
                    rabbitConnectionString: rabbitMq["Host"] ?? throw new NullReferenceException(),
                    name: "RabbitMq-check",
                    tags: ["rabbitmq", "messaging", "readiness"],
                    failureStatus: HealthStatus.Unhealthy
                );
        #endregion

        return services;
    }
}