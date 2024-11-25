using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using MassTransit;
using Application.Commons;
using Microsoft.AspNetCore.Mvc;
using NUlid;
using PRH_PaymentService_API.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentationDependencies(this IServiceCollection services,
        IConfiguration configuration)
    {
        #region Base-configuration

        services.AddControllers()
   .ConfigureApiBehaviorOptions(options =>
   {
       options.InvalidModelStateResponseFactory = context =>
       {
           // Create a list of ErrorDetail instances based on model state errors

           var errorDetails = context.ModelState
               .Where(ms => ms.Value.Errors.Count > 0)
               .SelectMany(ms => ms.Value.Errors
                   .Select(e => new ErrorDetail
                   {
                       Message = e.ErrorMessage,
                       Field = ms.Key
                   }))
               .ToList();

           // Create an instance of DetailBaseResponse to structure the response
           var response = new DetailBaseResponse<object>
           {
               Id = Ulid.NewUlid().ToString(),
               StatusCode = StatusCodes.Status422UnprocessableEntity,
               Message = "One or more validation errors occurred.",
               Success = false,
               Data = null,
               Errors = errorDetails,
               Timestamp = DateTime.UtcNow
           };

           // Return the structured response with a 422 status code
           return new ObjectResult(response)
           {
               StatusCode = StatusCodes.Status422UnprocessableEntity
           };
       };
   });

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

        services.AddHostedService<PaymentStatusPollingService>();

        return services;
    }
}