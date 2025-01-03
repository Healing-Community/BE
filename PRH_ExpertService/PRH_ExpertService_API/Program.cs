using System.Text.Json;
using Application;
using Application.Jobs;
using Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Persistence;
using PRH_ExpertService_API;
using PRH_ExpertService_API.Middleware;
using Prometheus;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

#region Thêm các dependency theo tầng

builder.Services.AddPresentationDependencies(builder.Configuration);
builder.Services.AddApplicationDependencies();
builder.Services.AddPersistenceDependencies();
builder.Services.AddInfrastructureDependencies(builder.Configuration);

#endregion

#region appsettings
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
#endregion

// Thêm dịch vụ gRPC
builder.Services.AddGrpc();

#region Quartz.NET

builder.Services.AddQuartz(q =>
{
    // Định nghĩa một job và trigger
    q.ScheduleJob<AppointmentStatusJob>(trigger => trigger
        .WithIdentity("AppointmentStatusJobTrigger")
        .StartNow()
        .WithCronSchedule("0 * * * * ?"));
});

// Thêm Quartz.NET Hosted Service
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

#endregion

var app = builder.Build();

#region Middleware

// Sử dụng middleware cho xác thực
app.UseMiddleware<AuthMiddleware>();

#endregion

#region Khởi động dịch vụ MassTransit

// Lấy và khởi động IBusControl cho MassTransit
var busControl = app.Services.GetRequiredService<IBusControl>();
await busControl.StartAsync();

#endregion

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", builder.Configuration["GeneralSettings:ApiName"]);
    c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
    c.RoutePrefix = "";
});

#region HealthChecks (Kiểm tra tình trạng hệ thống)
app.MapHealthChecks("/health/liveness", new HealthCheckOptions
{
    Predicate = (check) => check.Tags.Contains("liveness"),  // Chỉ lọc các liveness check
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
    Predicate = (check) => check.Tags.Contains("readiness"),  // Chỉ lọc các readiness check
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
#endregion

#region Prometheus (Dùng để thu thập metrics hệ thống)
app.UseHttpMetrics(); // Thu thập HTTP metrics
app.UseMetricServer(); // Expose metrics trên endpoint mặc định
#endregion

app.MapGrpcService<ExpertService>();

// Chỉ bật HTTPS Redirection trên môi trường Development
if (builder.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors();

app.UseAuthentication();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();