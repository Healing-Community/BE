using Application;
using Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Persistence;
using PRH_GroupService_API;
using PRH_GroupService_API.Middleware;
using Prometheus;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

#region Add-layer-dependencies

builder.Services.AddPresentationDependencies(builder.Configuration);
builder.Services.AddPersistenceDependencies();
builder.Services.AddInfrastructureDependencies(builder.Configuration);
builder.Services.AddApplicationDependencies();

# endregion

//builder.WebHost.ConfigureKestrel(options =>
//{
//    // Cấu hình lắng nghe HTTP/1.1 và HTTP/2 trên cổng 7098 với HTTPS
//    options.ListenLocalhost(7098, listenOptions =>
//    {
//        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
//        listenOptions.UseHttps(); // Sử dụng HTTPS
//    });
//});
// Cấu hình gRPC Service
builder.Services.AddGrpc();

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
// Map các service gRPC
app.MapGrpcService<GroupGrpcService>(); // Đăng ký GroupGrpcService cho gRPC
// Thêm endpoint để phục vụ các tệp tĩnh như index.html
app.UseStaticFiles();

app.MapFallbackToFile("index.html"); // Phục vụ index.html nếu truy cập từ URL

app.MapControllers();

app.Run();
