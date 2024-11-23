﻿using System.Text.Json;
using Application;
using Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Persistence;
using PRH_ExpertService_API;
using PRH_ExpertService_API.Middleware;
using PRH_ExpertService_API.Services;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Nạp cấu hình từ appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

#region Thêm các dependency theo tầng

builder.Services.AddPresentationDependencies(builder.Configuration);
builder.Services.AddApplicationDependencies();
builder.Services.AddPersistenceDependencies();
builder.Services.AddInfrastructureDependencies(builder.Configuration);

#endregion

// Cấu hình Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    // Lắng nghe trên Docker (môi trường Production)
    options.ListenAnyIP(81, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1; // Chỉ hỗ trợ HTTP trên Docker
    });

    // Lắng nghe trên Local (môi trường Development)
    if (builder.Environment.IsDevelopment())
    {
        options.ListenAnyIP(5005, listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http1AndHttp2; // Hỗ trợ cả HTTP/HTTPS
            listenOptions.UseHttps(); // Chỉ bật HTTPS trên môi trường Development
        });
    }
});

// Thêm dịch vụ gRPC
builder.Services.AddGrpc();

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
    c.RoutePrefix = "";
});

//#region HealthChecks (Kiểm tra tình trạng hệ thống)
// app.MapHealthChecks("/health/liveness", new HealthCheckOptions
// {
//     Predicate = (check) => check.Tags.Contains("liveness"),  // Chỉ lọc các liveness check
//     ResponseWriter = async (context, report) =>
//     {
//         context.Response.ContentType = "application/json";
//         var result = JsonSerializer.Serialize(new
//         {
//             status = report.Status.ToString(),
//             checks = report.Entries.Select(entry => new
//             {
//                 name = entry.Key,
//                 status = entry.Value.Status.ToString(),
//                 description = entry.Value.Description,
//                 duration = entry.Value.Duration.ToString()
//             }),
//             totalDuration = report.TotalDuration
//         });
//         await context.Response.WriteAsync(result);
//     }
// });

// app.MapHealthChecks("/health/readiness", new HealthCheckOptions
// {
//     Predicate = (check) => check.Tags.Contains("readiness"),  // Chỉ lọc các readiness check
//     ResponseWriter = async (context, report) =>
//     {
//         context.Response.ContentType = "application/json";
//         var result = JsonSerializer.Serialize(new
//         {
//             status = report.Status.ToString(),
//             checks = report.Entries.Select(entry => new
//             {
//                 name = entry.Key,
//                 status = entry.Value.Status.ToString(),
//                 description = entry.Value.Description,
//                 duration = entry.Value.Duration.ToString()
//             }),
//             totalDuration = report.TotalDuration
//         });
//         await context.Response.WriteAsync(result);
//     }
// });
//#endregion

//#region Prometheus (Dùng để thu thập metrics hệ thống)
// app.UseHttpMetrics(); // Thu thập HTTP metrics
// app.UseMetricServer(); // Expose metrics trên endpoint mặc định
//#endregion

// Chỉ bật HTTPS Redirection trên môi trường Development
if (builder.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGrpcService<ExpertServiceImpl>();

app.Run();