using System.Text.Json;
using Application;
using Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Persistence;
using PRH_UserService_API;
using PRH_UserService_API.Middleware;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

#region Add-layer-dependencies

builder.Services.AddPresentationDependencies(builder.Configuration);
builder.Services.AddApplicationDependencies();
builder.Services.AddPersistenceDependencies();
builder.Services.AddInfrastructureDependencies(builder.Configuration);

# endregion

var app = builder.Build();

#region Middleware

app.UseMiddleware<AuthMiddleware>();

#endregion

#region MassTransitHostedService

// Configure MassTransit bus control
var busControl = app.Services.GetRequiredService<IBusControl>();
await busControl.StartAsync();

#endregion

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", builder.Configuration["GeneralSettings:ApiName"]);
    c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
    c.RoutePrefix = "swagger";
});

# region HealthChecks

app.MapHealthChecks("/health/liveness", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("liveness"), // Lọc chỉ liveness checks
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
    Predicate = check => check.Tags.Contains("readiness"), // Lọc chỉ readiness checks
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

#region Prometheus-Metrics

app.UseHttpMetrics();

app.UseMetricServer();

#endregion

// // Cấu hình Forwarded Headers
// app.UseForwardedHeaders(new ForwardedHeadersOptions
// {
//     ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto,
    
//     // Tùy chọn thêm: Cho phép các header forwarded từ bất kỳ proxy nào (nếu không rõ proxy)
//     // Xác định rõ proxy nếu cần bằng app.ForwardedHeadersOptions.KnownProxies hoặc KnownNetworks
//     ForwardLimit = null // không giới hạn số header forwarded
// });

app.UseHttpsRedirection();

app.UseCors();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();