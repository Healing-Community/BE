﻿using System.Text.Json;
using Application;
using Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Persistence;
using PRH_PaymentService_API.Services;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Nạp cấu hình từ appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

#region Add-layer-dependencies

builder.Services.AddPresentationDependencies(builder.Configuration);
builder.Services.AddApplicationDependencies(builder.Configuration);
builder.Services.AddPersistenceDependencies();
builder.Services.AddInfrastructureDependencies(builder.Configuration);

# endregion

// Lấy URL từ cấu hình và đăng ký ExpertServiceGrpcClient
var expertServiceUrl = builder.Configuration["ExpertServiceUrl"];
if (string.IsNullOrEmpty(expertServiceUrl))
{
    throw new ArgumentException("ExpertServiceUrl không được cấu hình trong appsettings.json.");
}
builder.Services.AddSingleton(sp => new ExpertServiceGrpcClient(expertServiceUrl));

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(GetPaymentDetailsQueryHandler).Assembly);
});

var app = builder.Build();

#region Middleware

app.UseMiddleware<AuthMiddleware>();

#endregion

#region MassTransitHostedService

var busControl = app.Services.GetRequiredService<IBusControl>();
await busControl.StartAsync();

#endregion

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", builder.Configuration["GeneralSettings:ApiName"]);
    c.RoutePrefix = "";
});

# region HealthChecks
app.MapHealthChecks("/health/liveness", new HealthCheckOptions
{
    Predicate = (check) => check.Tags.Contains("liveness"),  // Lọc chỉ liveness checks
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                duration = entry.Value.Duration.ToString()
            }),
            totalDuration = report.TotalDuration
        });
        await context.Response.WriteAsync(result);
    }
});

app.MapHealthChecks("/health/readiness", new HealthCheckOptions
{
    Predicate = (check) => check.Tags.Contains("readiness"),  // Lọc chỉ readiness checks
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                duration = entry.Value.Duration.ToString()
            }),
            totalDuration = report.TotalDuration
        });
        await context.Response.WriteAsync(result);
    }
});
# endregion

#region Prometheus

app.UseHttpMetrics();

app.UseMetricServer();

#endregion

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();