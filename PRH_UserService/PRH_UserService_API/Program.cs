using Infrastructure;
using Application;
using Persistence;
using PRH_UserService_API;
using PRH_UserService_API.Middleware;

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
app.UseMiddleware<ConvertForbiddenToUnauthorizedMiddleware>();

#endregion

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PRH UserService API");
    c.RoutePrefix = "";
});
app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();