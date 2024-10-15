﻿using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MassTransit;

namespace PRH_PostService_API;

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
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(rabbitMq["Host"] ?? throw new NullReferenceException()), h =>
                {
                    h.Username(rabbitMq["Username"] ?? throw new NullReferenceException());
                    h.Password(rabbitMq["Password"] ?? throw new NullReferenceException());
                });
                // Thiết lập Retry
                cfg.UseMessageRetry(retryConfig =>
                {
                    retryConfig.Interval(5, TimeSpan.FromSeconds(5)); // Thử lại 5 lần, mỗi lần cách nhau 10 giây
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

        return services;
    }
}