
using Application;
using Domain.Constants;
using Infrastructure;
using MassTransit;
using Persistence;
using PRH_NotificationService_API.Consumer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

# region Layer Dependencies
builder.Services.AddApplicationDependencies();
builder.Services.AddPersistenceDependencies();
builder.Services.AddInfrastructureDependencies(builder.Configuration);
# endregion

#region Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "PRH NotificationSerive API", Version = "v1" });
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

#region AMQP
builder.Services.AddMassTransit(x =>
{
    builder.Services.AddMassTransit(x =>
    {
        x.AddConsumer<PostServiceConsumer>();
        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(new Uri(builder.Configuration["RabbitMq:Host"] ?? throw new NullReferenceException()), h =>
            {
                h.Username(builder.Configuration["RabbitMq:Username"] ?? throw new NullReferenceException());
                h.Password(builder.Configuration["RabbitMq:Password"] ?? throw new NullReferenceException());
            });

            // Đăng ký consumer
            cfg.ReceiveEndpoint(QueueName.PostQueue.ToString(), c =>
            {
                c.ConfigureConsumer<PostServiceConsumer>(context);
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
    builder.Services.AddHostedService<MassTransitHostedService>();
});

// Add MassTransit hosted service
builder.Services.AddHostedService<MassTransitHostedService>();

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
