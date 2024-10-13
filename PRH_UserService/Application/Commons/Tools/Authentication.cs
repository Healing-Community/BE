using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

namespace Application.Commons.Tools;

public static class Authentication
{
    public static string GetUserIdFromHttpContext(HttpContext httpContext)
    {
        if (!httpContext.Request.Headers.ContainsKey("Authorization")) throw new Exception("Need Authorization");

        string? authorizationHeader = httpContext.Request.Headers["Authorization"];

        if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            throw new Exception($"Invalid authorization header: {authorizationHeader}");

        var jwtToken = authorizationHeader["Bearer ".Length..];
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(jwtToken);
        var idClaim = token.Claims.FirstOrDefault(claim =>
            claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

        return idClaim?.Value ?? throw new Exception("Cannot get userId from token");
    }
}