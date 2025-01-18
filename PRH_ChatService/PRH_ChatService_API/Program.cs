using Application;
using Domain.Entities;
using Infrastructure;
using MassTransit;
using Persistence;
using PRH_ChatService_API;
using PRH_ChatService_API.Middleware;

var builder = WebApplication.CreateBuilder(args);

#region Add-layer-dependencies

builder.Services.AddPresentationDependencies(builder.Configuration);
builder.Services.AddApplicationDependencies();
builder.Services.AddPersistenceDependencies();
builder.Services.AddInfrastructureDependencies(builder.Configuration);

# endregion

# region appsettings
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
# endregion

var app = builder.Build();

Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");

#region Middleware

app.UseMiddleware<AuthMiddleware>();

// Enable WebSocket middleware
app.UseWebSockets();
app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();

        using (var scope = app.Services.CreateScope())
        {
            // Use scoped services here
            var chatMessagesRepository = scope.ServiceProvider.GetRequiredService<IMongoRepository<ChatMessages>>();
            var webSocketHandler = new WebSocketHandler(chatMessagesRepository);

            await webSocketHandler.HandleChatSessionAsync(context, webSocket);
        }
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});



#endregion

#region MassTransitHostedService

//// Configure MassTransit bus control
//var busControl = app.Services.GetRequiredService<IBusControl>();
//await busControl.StartAsync();

#endregion

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", builder.Configuration["GeneralSettings:ApiName"]);
    c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
    c.RoutePrefix = "";
});

app.UseHttpsRedirection();

app.UseCors();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();