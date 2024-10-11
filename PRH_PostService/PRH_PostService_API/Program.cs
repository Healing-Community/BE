using Application;
using Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddPersistenceDependencies();
builder.Services.AddInfrastructureDependencies(builder.Configuration);
builder.Services.AddApplicationDependencies();
#region Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "PRH PostSerive API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement{
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
#endregion

#region Authorization-Authentication
builder.Services.AddAuthorization();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"] ?? "")),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
    };
});
#endregion

#region AMQP

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(builder.Configuration["RabbitMq:Host"] ?? throw new NullReferenceException()), h =>
        {
            h.Username(builder.Configuration["RabbitMq:Username"] ?? throw new NullReferenceException());
            h.Password(builder.Configuration["RabbitMq:Password"] ?? throw new NullReferenceException());
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
builder.Services.AddHostedService<MassTransitHostedService>();

#endregion

var app = builder.Build();

#region MassTransitHostedService

// Configure MassTransit bus control
var busControl = app.Services.GetRequiredService<IBusControl>();
await busControl.StartAsync();

#endregion
app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
