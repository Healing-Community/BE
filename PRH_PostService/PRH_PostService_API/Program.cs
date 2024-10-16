using Application;
using Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using PRH_PostService_API;
using PRH_PostService_API.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Add-layer-dependencies

builder.Services.AddPresentationDependencies(builder.Configuration);
builder.Services.AddPersistenceDependencies();
builder.Services.AddInfrastructureDependencies(builder.Configuration);
builder.Services.AddApplicationDependencies();

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
    c.RoutePrefix = "";
});

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
