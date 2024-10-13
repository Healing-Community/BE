using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Microsoft.IdentityModel.Tokens;

namespace PRH_UserService_API.Middleware;

public class AuthMiddleware(RequestDelegate next, IConfiguration configuration)
{
    private readonly IConfiguration _configuration = configuration;

    public async Task Invoke(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var allowAnonymousRefreshTokenAttribute = endpoint?.Metadata.GetMetadata<AllowAnonymousRefreshTokenAttribute>();
        if (allowAnonymousRefreshTokenAttribute != null)
        {
            await next(context);
            return;
        }

        // Token validation logic...
        if (context.Request.Headers.ContainsKey("Authorization"))
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(token))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                try
                {
                    var jwtToken = tokenHandler.ReadJwtToken(token);
                    if (jwtToken.ValidTo < DateTime.UtcNow)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        await context.Response.WriteAsync("Token has expired");
                        return;
                    }
                }
                catch (SecurityTokenException)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("Invalid token");
                    return;
                }
            }
        }

        await next(context);
    }
}