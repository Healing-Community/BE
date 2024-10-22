using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public AuthMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var allowAnonymous = endpoint?.Metadata.GetMetadata<AllowAnonymous>();
        if (allowAnonymous != null)
        {
            await _next(context);
            return;
        }

        if (context.Request.Headers.ContainsKey("Authorization"))
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(token))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                try
                {
                    // Retrieve the secret key from appsettings.json
                    var jwtSettings = _configuration.GetSection("JwtSettings");


                    // Set up token validation parameters
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(jwtSettings["Secret"] ?? throw new InvalidOperationException())),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = true
                    };

                    // Validate the token
                    tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                    // You can further inspect the validatedToken if needed
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

        await _next(context);
    }
}
