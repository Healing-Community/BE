using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace PRH_PostService_API.Middleware
{
    public class AuthMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        public async Task Invoke(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var allowAnonymous = endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>();
            if (allowAnonymous != null)
            {
                await next(context);
                return;
            }

            if (context.Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues value))
            {
                var token = value.ToString().Replace("Bearer ", "");
                if (!string.IsNullOrEmpty(token))
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    try
                    {
                        var jwtSettings = configuration.GetSection("JwtSettings");
                        var validationParameters = new TokenValidationParameters
                        {
                            ValidIssuer = jwtSettings["Issuer"],
                            ValidAudience = jwtSettings["Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey
                                (Encoding.UTF8.GetBytes(jwtSettings["Secret"] ?? throw new InvalidOperationException())),
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateIssuerSigningKey = true,
                            ValidateLifetime = false // Disable automatic lifetime validation
                        };

                        // Validate the token without lifetime check
                        var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                        var jwtToken = validatedToken as JwtSecurityToken;

                        // Extract expiration time and compare with current time
                        var expiration = jwtToken?.ValidTo;
                        if (expiration.HasValue && expiration.Value < DateTime.UtcNow)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            await context.Response.WriteAsync("Token has expired");
                            return;
                        }

                        // Proceed if token is valid and not expired
                    }
                    catch (SecurityTokenException)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        await context.Response.WriteAsync("Invalid token or secret key mismatch");
                        return;
                    }
                    catch (Exception)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        await context.Response.WriteAsync("Token validation failed");
                        return;
                    }
                }
            }

            await next(context);
        }
    }
}